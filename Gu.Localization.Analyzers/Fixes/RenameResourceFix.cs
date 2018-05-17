namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Rename;

    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RenameResourceFix))]
    internal class RenameResourceFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            GULOC07KeyDoesNotMatch.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => null;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor<PropertyDeclarationSyntax>(diagnostic, out var propertyDeclaration) &&
                    semanticModel.TryGetSymbol(propertyDeclaration, context.CancellationToken, out var property) &&
                    diagnostic.Properties.TryGetValue("Key", out var name) &&
                    !property.ContainingType.TryFindFirstMember(name, out _) &&
                    Resources.TryGetDefaultResx(property.ContainingType, out _))
                {
                    context.RegisterCodeFix(
                        new PreviewCodeAction(
                            "Rename resource",
                            cancellationToken => Renamer.RenameSymbolAsync(context.Document.Project.Solution, property, name, null, cancellationToken),
                            cancellationToken => RenameAsync(context.Document, property, name, cancellationToken)),
                        diagnostic);
                }
            }
        }

        private static async Task<Solution> RenameAsync(Document document, IPropertySymbol property, string newName, CancellationToken cancellationToken)
        {
            if (Resources.TryGetDefaultResx(property.ContainingType, out var resx))
            {
                UpdateResx(resx, property, newName);
                foreach (var cultureResx in resx.Directory.EnumerateFiles($"{Path.GetFileNameWithoutExtension(resx.Name)}.*.resx", SearchOption.TopDirectoryOnly))
                {
                    UpdateResx(cultureResx, property, newName);
                }

                UpdateXaml(document.Project, property, newName);
                foreach (var project in document.Project.ReferencingProjects())
                {
                    UpdateXaml(project, property, newName);
                }

                var solution = await Renamer.RenameSymbolAsync(document.Project.Solution, property, newName, null, cancellationToken);
                if (property.TrySingleDeclaration(cancellationToken, out PropertyDeclarationSyntax declaration))
                {
                    var root = await document.GetSyntaxRootAsync(cancellationToken);
                    return solution.WithDocumentSyntaxRoot(
                        document.Id,
                        root.ReplaceNode(
                            declaration,
                            Property.Rewrite(declaration, newName)));
                }

                return solution;
            }

            return document.Project.Solution;
        }

        private static void UpdateResx(FileInfo resx, IPropertySymbol property, string newName)
        {
            var textWithEncoding = TextWithEncoding.Create(resx.FullName);
            var xDocument = XDocument.Parse(textWithEncoding.Text);
            if (xDocument.Root is XElement root)
            {
                foreach (var candidate in root.Elements("data"))
                {
                    if (candidate.Attribute("name") is XAttribute attribute &&
                        attribute.Value == property.Name)
                    {
                        attribute.Value = newName;
                        using (var writer = new StreamWriter(File.Open(resx.FullName, FileMode.Create, FileAccess.Write), textWithEncoding.Encoding))
                        {
                            xDocument.Save(writer);
                        }
                    }
                }
            }
        }

        private static void UpdateXaml(Project project, IPropertySymbol property, string newName)
        {
            if (project.MetadataReferences.TryFirst(x => x.Display.EndsWith("System.Xaml.dll"), out _))
            {
                var directory = Path.GetDirectoryName(project.FilePath);
                var csprojText = File.ReadAllText(project.FilePath);
                if (Regex.IsMatch(csprojText, "<TargetFrameworks?\b"))
                {
                    var csproj = XDocument.Parse(csprojText);
                    if (csproj.Root is XElement root)
                    {
                        foreach (var page in root.Descendants().Where(x => x.Name.LocalName == "Page"))
                        {
                            if (page.Attribute("Include") is XAttribute attribute &&
                                attribute.Value.EndsWith(".xaml"))
                            {
                                var xamlFile = Path.Combine(directory, attribute.Value);
                                Update(xamlFile);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var xamlFile in Directory.EnumerateFiles(directory, "*.xaml", SearchOption.AllDirectories))
                    {
                        Update(xamlFile);
                    }
                }
            }

            void Update(string fileName)
            {
                var xaml = TextWithEncoding.Create(fileName);
                var pattern = $"xmlns:(?<alias>\\w+)=\"clr-namespace:{property.ContainingType.ContainingSymbol}\"";
                if (Regex.Match(xaml.Text, pattern) is Match match &&
                    match.Success)
                {
                    var updated = xaml.Text.Replace(
                        $"{match.Groups["alias"].Value}:{property.ContainingType.Name}.{property.Name}",
                        $"{match.Groups["alias"].Value}:{property.ContainingType.Name}.{newName}");
                    if (updated != xaml.Text)
                    {
                        File.WriteAllText(fileName, updated, xaml.Encoding);
                    }
                }
            }
        }

        private struct TextWithEncoding
        {
            public TextWithEncoding(string text, Encoding encoding)
            {
                this.Text = text;
                this.Encoding = encoding;
            }

            public string Text { get; }

            public Encoding Encoding { get; }

            public static TextWithEncoding Create(string fileName)
            {
                using (var reader = new StreamReader(File.OpenRead(fileName), Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                {
                    return new TextWithEncoding(reader.ReadToEnd(), reader.CurrentEncoding);
                }
            }
        }

        private class Property : CSharpSyntaxRewriter
        {
            private readonly string newValue;

            private Property(string newValue)
            {
                this.newValue = newValue;
            }

            public static PropertyDeclarationSyntax Rewrite(PropertyDeclarationSyntax declaration, string newValue)
            {
                return (PropertyDeclarationSyntax)new Property(newValue).Visit(declaration);
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                if (token.Parent is PropertyDeclarationSyntax propertyDeclaration &&
                    propertyDeclaration.Identifier == token)
                {
                    return SyntaxFactory.Identifier(token.LeadingTrivia, this.newValue, token.TrailingTrivia);
                }

                return base.VisitToken(token);
            }

            public override SyntaxNode VisitArgument(ArgumentSyntax node)
            {
                if (node.Parent is ArgumentListSyntax argumentList &&
                    argumentList.Arguments.IndexOf(node) == 0 &&
                    argumentList.Parent is InvocationExpressionSyntax invocation &&
                    invocation.TryGetMethodName(out var method) &&
                    method == "GetString")
                {
                    return node.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(this.newValue)));
                }

                return base.VisitArgument(node);
            }

            public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                if (node.IsKind(SyntaxKind.StringLiteralExpression) &&
                    node.Parent is ArgumentSyntax argument &&
                    argument.Parent is ArgumentListSyntax argumentList &&
                    argumentList.Parent is InvocationExpressionSyntax invocation &&
                    invocation.TryGetMethodName(out var method) &&
                    method == "GetString")
                {
                    return node.WithToken(SyntaxFactory.Literal(this.newValue));
                }

                return base.VisitLiteralExpression(node);
            }
        }
    }
}
