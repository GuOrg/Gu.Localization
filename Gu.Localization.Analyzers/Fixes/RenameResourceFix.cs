namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.IO;
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
                    !property.ContainingType.TryFindFirstMember(name, out _))
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

        private static async Task<Solution> RenameAsync(Document document, IPropertySymbol property, string name, CancellationToken cancellationToken)
        {
            if (Resources.TryGetDefaultResx(property.ContainingType, out var resx))
            {
                RenameKey(resx, property.Name, name);
                foreach (var cultureResx in resx.Directory.EnumerateFiles($"{Path.GetFileNameWithoutExtension(resx.Name)}.*.resx", SearchOption.TopDirectoryOnly))
                {
                    RenameKey(cultureResx, property.Name, name);
                }

                var solution = await Renamer.RenameSymbolAsync(document.Project.Solution, property, name, null, cancellationToken);
                if (property.TrySingleDeclaration(cancellationToken, out PropertyDeclarationSyntax declaration))
                {
                    var root = await document.GetSyntaxRootAsync(cancellationToken);
                    return solution.WithDocumentSyntaxRoot(
                        document.Id,
                        root.ReplaceNode(
                            declaration,
                            Property.Rewrite(declaration, name)));
                }

                return solution;
            }

            return document.Project.Solution;
        }

        private static void RenameKey(FileInfo resx, string oldName, string newName)
        {
            var xDocument = XDocument.Load(resx.FullName);
            if (xDocument.Root is XElement root)
            {
                foreach (var candidate in root.Elements("data"))
                {
                    if (candidate.Attribute("name") is XAttribute attribute &&
                        attribute.Value == oldName)
                    {
                        attribute.Value = newName;
                        using (var stream = File.OpenWrite(resx.FullName))
                        {
                            xDocument.Save(stream);
                        }
                    }
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
                    return SyntaxFactory.ParseToken(this.newValue);
                }

                return base.VisitToken(token);
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
                    return node.WithToken(SyntaxFactory.ParseToken($"\"{this.newValue}\""));
                }

                return base.VisitLiteralExpression(node);
            }
        }
    }
}
