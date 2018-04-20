namespace Gu.Localization.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseResourceFix))]
    internal class UseResourceFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            UseResource.DiagnosticId);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                                   .ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is LiteralExpressionSyntax literal &&
                    TryGetKey(literal.Token.ValueText, out var key) &&
                    context.Document.Project.Documents.TrySingle(x => x.Name == "Resources.Designer.cs", out var resources) &&
                    Path.Combine(Path.GetDirectoryName(context.Document.Project.FilePath), "Properties\\Resources.resx") is string resx &&
                    File.Exists(resx))
                {
                    if (PropertyWalker.Contains(resources, key))
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                "Use existing resource in Translator.Translate",
                                _ => Task.FromResult(
                                    context.Document.WithSyntaxRoot(
                                        syntaxRoot.ReplaceNode(
                                            literal,
                                            SyntaxFactory.ParseExpression(
                                                    $"Gu.Localization.Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.{key}))")
                                                .WithSimplifiedNames()))),
                                "Move to resource and use Translator.Translate"),
                            diagnostic);
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                "Use existing resource.",
                                _ => Task.FromResult(
                                    context.Document.WithSyntaxRoot(
                                        syntaxRoot.ReplaceNode(
                                            literal,
                                            SyntaxFactory.ParseExpression($"Properties.Resources.{key}")
                                                .WithSimplifiedNames()))),
                                "Use existing resource."),
                            diagnostic);
                    }
                    else
                    {
                        context.RegisterCodeFix(
                            new PreviewCodeAction(
                                "Move to resources and use Translator.Translate.",
                                _ => Task.FromResult(context.Document.WithSyntaxRoot(
                                    syntaxRoot.ReplaceNode(
                                        literal,
                                        SyntaxFactory.ParseExpression($"Gu.Localization.Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.{key}))")
                                            .WithSimplifiedNames()))),
                                _ => AddResourceAndReplaceAsync(
                                    context.Document,
                                    literal,
                                    SyntaxFactory.ParseExpression($"Gu.Localization.Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.{key}))")
                                        .WithSimplifiedNames(),
                                    resx,
                                    key),
                                "Move to resources and use Translator.Translate."),
                            diagnostic);

                        context.RegisterCodeFix(
                            new PreviewCodeAction(
                                "Move to resources.",
                                _ => Task.FromResult(
                                    context.Document.WithSyntaxRoot(
                                        syntaxRoot.ReplaceNode(
                                            literal,
                                            SyntaxFactory.ParseExpression($"Properties.Resources.{key}")
                                                         .WithSimplifiedNames()))),
                                _ => AddResourceAndReplaceAsync(
                                    context.Document,
                                    literal,
                                    SyntaxFactory.ParseExpression($"Properties.Resources.{key}")
                                                 .WithSimplifiedNames(),
                                    resx,
                                    key),
                                "Move to resources."),
                            diagnostic);
                    }
                }
            }
        }

        private static Task<Document> AddResourceAndReplaceAsync(Document document, LiteralExpressionSyntax literal, ExpressionSyntax expression, string resx, string key)
        {
            var xElement = new XElement("data");
            xElement.Add(new XAttribute("name", key));
            xElement.Add(new XAttribute(XNamespace.Xml + "space", "preserve"));
            xElement.Add(new XElement("value", literal.Token.ValueText));
            var xDocument = XDocument.Load(resx);
            xDocument.Root.Add(xElement);
            using (var stream = File.OpenWrite(resx))
            {
                xDocument.Save(stream);
            }

            var designer = Path.Combine(Path.GetDirectoryName(document.Project.FilePath), "Properties\\Resources.Designer.cs");
            if (File.Exists(designer) &&
                File.ReadAllLines(designer).ToList() is List<string> lines &&
                lines.Count > 3)
            {
                // Adding a temp key so that we don't have a build error until next gen.
                // internal static string Key => ResourceManager.GetString("Key", resourceCulture);
                lines.Insert(
                    lines.Count - 2,
                    $"        internal static string {key} => ResourceManager.GetString(\"{key}\", resourceCulture);");
                File.WriteAllLines(designer, lines);
            }

            if (document.TryGetSyntaxRoot(out var root))
            {
                return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(literal, expression)));
            }

            return Task.FromResult(document);
        }

        private static bool TryGetKey(string text, out string key)
        {
            key = Regex.Replace(text, "{(?<n>\\d+)}", x => $"__{x.Groups["n"].Value}__")
                       .Replace(" ", "_")
                       .Replace(".", "_");

            if (char.IsDigit(key[0]))
            {
                key = "_" + key;
            }

            return SyntaxFacts.IsValidIdentifier(key);
        }

        private class PropertyWalker : CSharpSyntaxWalker
        {
            private readonly string property;
            private bool foundProperty;

            public PropertyWalker(string property)
            {
                this.property = property;
            }

            public static bool Contains(Document resources, string property)
            {
                return resources.TryGetSyntaxTree(out var tree) &&
                       tree.TryGetRoot(out var root) &&
                       Contains(root, property);
            }

            public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                if (node.Identifier.ValueText == this.property)
                {
                    this.foundProperty = true;
                }

                base.VisitPropertyDeclaration(node);
            }

            private static bool Contains(SyntaxNode node, string property)
            {
                var walker = new PropertyWalker(property);
                walker.Visit(node);
                return walker.foundProperty;
            }
        }

        private class PreviewCodeAction : CodeAction
        {
            private readonly CodeAction preview;
            private readonly CodeAction change;

            public PreviewCodeAction(
                string title,
                Func<CancellationToken, Task<Document>> preview,
                Func<CancellationToken, Task<Document>> change,
                string equivalenceKey)
            {
                this.Title = title;
                this.preview = Create(title, preview, equivalenceKey);
                this.change = Create(title, change, equivalenceKey);
                this.EquivalenceKey = equivalenceKey;
            }

            public override string Title { get; }

            public override string EquivalenceKey { get; }

            protected override async Task<IEnumerable<CodeActionOperation>> ComputePreviewOperationsAsync(CancellationToken cancellationToken)
            {
                return await this.preview.GetOperationsAsync(cancellationToken);
            }

            protected override async Task<IEnumerable<CodeActionOperation>> ComputeOperationsAsync(CancellationToken cancellationToken)
            {
                return await this.change.GetOperationsAsync(cancellationToken);
            }
        }
    }
}
