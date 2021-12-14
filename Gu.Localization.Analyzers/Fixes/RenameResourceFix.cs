namespace Gu.Localization.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.IO;
    using System.Linq;
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
            Descriptors.GULOC07KeyDoesNotMatch.Id);

        public override FixAllProvider? GetFixAllProvider() => null;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot is { } &&
                    syntaxRoot.TryFindNodeOrAncestor<PropertyDeclarationSyntax>(diagnostic, out var propertyDeclaration) &&
                    semanticModel is { } &&
                    semanticModel.TryGetSymbol(propertyDeclaration, context.CancellationToken, out var property) &&
                    diagnostic.Properties.TryGetValue("Key", out var name) &&
                    !property.ContainingType.TryFindFirstMember(name!, out _) &&
                    ResxFile.TryGetDefault(property.ContainingType, out _))
                {
                    context.RegisterCodeFix(
                        new PreviewCodeAction(
                            "Rename resource",
                            cancellationToken => Renamer.RenameSymbolAsync(context.Document.Project.Solution, property, name!, context.Document.Project.Solution.Options, cancellationToken),
                            cancellationToken => RenameAsync(context.Document, property, name!, cancellationToken)),
                        diagnostic);
                }
            }
        }

        private static async Task<Solution> RenameAsync(Document document, IPropertySymbol property, string newName, CancellationToken cancellationToken)
        {
            if (ResxFile.TryGetDefault(property.ContainingType, out var resx))
            {
                resx.RenameKey(property.Name, newName);
                foreach (var cultureResx in resx.CultureSpecific())
                {
                    cultureResx.RenameKey(property.Name, newName);
                }

                UpdateXaml(document.Project, property, newName);
                foreach (var project in document.Project.ReferencingProjects())
                {
                    UpdateXaml(project, property, newName);
                }

                var solution = await Renamer.RenameSymbolAsync(document.Project.Solution, property, newName, document.Project.Solution.Options, cancellationToken).ConfigureAwait(false);
                if (property.TrySingleDeclaration(cancellationToken, out PropertyDeclarationSyntax? declaration))
                {
                    var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                    return solution.WithDocumentSyntaxRoot(
                        document.Id,
                        root!.ReplaceNode(declaration, Property.Rewrite(declaration, newName))!);
                }

                return solution;
            }

            return document.Project.Solution;
        }

        private static void UpdateXaml(Project project, IPropertySymbol property, string newName)
        {
            if (project.MetadataReferences.TryFirst(x => x.Display!.EndsWith("System.Xaml.dll", StringComparison.Ordinal), out _) &&
                project.FilePath is { } filePath &&
                Path.GetDirectoryName(filePath) is { } directory)
            {
                var csprojText = File.ReadAllText(filePath);
                if (csprojText.Contains("Sdk=\"Microsoft.NET.Sdk"))
                {
                    foreach (var fileName in Directory.EnumerateFiles(directory, "*.xaml", SearchOption.AllDirectories))
                    {
                        if (XamlFile.TryUpdateUsage(fileName, property, newName, out var xamlFile))
                        {
                            File.WriteAllText(fileName, xamlFile.Text, xamlFile.Encoding);
                        }
                    }
                }
                else
                {
                    var csproj = XDocument.Parse(csprojText);
                    if (csproj.Root is { } root)
                    {
                        foreach (var page in root.Descendants().Where(x => x.Name.LocalName == "Page"))
                        {
                            if (page.Attribute("Include") is { } attribute &&
                                attribute.Value.EndsWith(".xaml", StringComparison.Ordinal))
                            {
                                var fileName = Path.Combine(directory, attribute.Value);
                                if (XamlFile.TryUpdateUsage(fileName, property, newName, out var xamlFile))
                                {
                                    File.WriteAllText(fileName, xamlFile.Text, xamlFile.Encoding);
                                }
                            }
                        }
                    }
                }
            }
        }

        private class Property : CSharpSyntaxRewriter
        {
            private readonly string newKey;

            private Property(string newKey)
            {
                this.newKey = newKey;
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                if (token.Parent is PropertyDeclarationSyntax propertyDeclaration &&
                    propertyDeclaration.Identifier == token)
                {
                    return SyntaxFactory.Identifier(token.LeadingTrivia, this.newKey, token.TrailingTrivia);
                }

                return base.VisitToken(token);
            }

            public override SyntaxNode VisitArgument(ArgumentSyntax node)
            {
                if (node.Parent is ArgumentListSyntax { Parent: InvocationExpressionSyntax invocation } argumentList &&
                    argumentList.Arguments.IndexOf(node) == 0 &&
                    invocation.TryGetMethodName(out var method) &&
                    method == "GetString")
                {
                    return node.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(this.newKey)));
                }

                return base.VisitArgument(node)!;
            }

            public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                if (node.IsKind(SyntaxKind.StringLiteralExpression) &&
                    node.Parent is ArgumentSyntax { Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax invocation } } &&
                    invocation.TryGetMethodName(out var method) &&
                    method == "GetString")
                {
                    return node.WithToken(SyntaxFactory.Literal(this.newKey));
                }

                return base.VisitLiteralExpression(node)!;
            }

            internal static PropertyDeclarationSyntax Rewrite(PropertyDeclarationSyntax declaration, string newValue)
            {
                return (PropertyDeclarationSyntax)new Property(newValue).Visit(declaration);
            }
        }
    }
}
