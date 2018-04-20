namespace Gu.Localization.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseResourceFix))]
    internal class UseResourceFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            GULOC03UseResource.DiagnosticId);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (await context.Document.GetSyntaxRootAsync(context.CancellationToken) is SyntaxNode syntaxRoot &&
                await context.Document.GetSemanticModelAsync(context.CancellationToken) is SemanticModel semanticModel)
            {
                foreach (var diagnostic in context.Diagnostics)
                {
                    if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is LiteralExpressionSyntax literal &&
                        TryGetKey(literal.Token.ValueText, out var key))
                    {
                        foreach (var resourcesType in semanticModel.LookupResourceTypes(diagnostic.Location.SourceSpan.Start, name: "Resources"))
                        {
                            if (resourcesType.TryFindProperty(key, out _))
                            {
                                var memberAccess = resourcesType.ToMinimalDisplayString(semanticModel, literal.SpanStart, SymbolDisplayFormat.MinimallyQualifiedFormat);
                                if (TryFindCustomTranslate(resourcesType, out var customTranslate))
                                {
                                    var translateKey = $"{customTranslate.ContainingType.ToMinimalDisplayString(semanticModel, literal.SpanStart, SymbolDisplayFormat.MinimallyQualifiedFormat)}.{customTranslate.Name}";
                                    context.RegisterCodeFix(
                                        CodeAction.Create(
                                            $"Use existing {memberAccess}.{key} in {translateKey}({memberAccess}.{key})",
                                            _ => Task.FromResult(
                                                context.Document.WithSyntaxRoot(
                                                    syntaxRoot.ReplaceNode(
                                                        literal,
                                                        SyntaxFactory
                                                            .ParseExpression(
                                                                $"{translateKey}(nameof({memberAccess}.{key}))")
                                                            .WithSimplifiedNames())))),
                                        diagnostic);
                                }

                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        $"Use existing {memberAccess}.{key} in Translator.Translate",
                                        _ => Task.FromResult(
                                            context.Document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    literal,
                                                    SyntaxFactory
                                                        .ParseExpression(
                                                            $"Gu.Localization.Translator.Translate({memberAccess}.ResourceManager, nameof({memberAccess}.{key}))")
                                                        .WithSimplifiedNames())))),
                                    diagnostic);
                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        $"Use existing {memberAccess}.{key}.",
                                        _ => Task.FromResult(
                                            context.Document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    literal,
                                                    SyntaxFactory.ParseExpression($"{memberAccess}.{key}")
                                                                 .WithSimplifiedNames())))),
                                    diagnostic);
                            }
                            else if (TryGetResx(resourcesType, out var resx))
                            {
                                var memberAccess = resourcesType.ToMinimalDisplayString(semanticModel, literal.SpanStart, SymbolDisplayFormat.MinimallyQualifiedFormat);
                                if (TryFindCustomTranslate(resourcesType, out var customTranslate))
                                {
                                    var translateKey = $"{customTranslate.ContainingType.ToMinimalDisplayString(semanticModel, literal.SpanStart, SymbolDisplayFormat.MinimallyQualifiedFormat)}.{customTranslate.Name}";
                                    context.RegisterCodeFix(
                                        new PreviewCodeAction(
                                            $"Move to {memberAccess}.{key} and use {translateKey}({memberAccess}.{key}).",
                                            _ => Task.FromResult(
                                                context.Document.WithSyntaxRoot(
                                                    syntaxRoot.ReplaceNode(
                                                        literal,
                                                        SyntaxFactory
                                                            .ParseExpression(
                                                                $"{translateKey}(nameof({memberAccess}.{key}))")
                                                            .WithSimplifiedNames()))),
                                            cancellationToken => AddResourceAndReplaceAsync(
                                                context.Document,
                                                literal,
                                                SyntaxFactory.ParseExpression(
                                                        $"{translateKey}(nameof({memberAccess}.{key}))")
                                                    .WithSimplifiedNames(),
                                                resx,
                                                key,
                                                cancellationToken)),
                                        diagnostic);
                                }

                                context.RegisterCodeFix(
                                    new PreviewCodeAction(
                                        $"Move to {memberAccess}.{key} and use Translator.Translate.",
                                        _ => Task.FromResult(
                                            context.Document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    literal,
                                                    SyntaxFactory
                                                        .ParseExpression(
                                                            $"Gu.Localization.Translator.Translate({memberAccess}.ResourceManager, nameof({memberAccess}.{key}))")
                                                        .WithSimplifiedNames()))),
                                        cancellationToken => AddResourceAndReplaceAsync(
                                            context.Document,
                                            literal,
                                            SyntaxFactory.ParseExpression(
                                                    $"Gu.Localization.Translator.Translate({memberAccess}.ResourceManager, nameof({memberAccess}.{key}))")
                                                .WithSimplifiedNames(),
                                            resx,
                                            key,
                                            cancellationToken)),
                                    diagnostic);

                                context.RegisterCodeFix(
                                    new PreviewCodeAction(
                                        $"Move to {memberAccess}.{key}.",
                                        _ => Task.FromResult(
                                            context.Document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    literal,
                                                    SyntaxFactory.ParseExpression($"{memberAccess}.{key}")
                                                        .WithSimplifiedNames()))),
                                        cancellationToken => AddResourceAndReplaceAsync(
                                            context.Document,
                                            literal,
                                            SyntaxFactory.ParseExpression($"{memberAccess}.{key}")
                                                .WithSimplifiedNames(),
                                            resx,
                                            key,
                                            cancellationToken)),
                                    diagnostic);
                            }
                        }
                    }
                }
            }
        }

        private static async Task<Solution> AddResourceAndReplaceAsync(Document document, LiteralExpressionSyntax literal, ExpressionSyntax expression, FileInfo resx, string key, CancellationToken cancellationToken)
        {
            var xElement = new XElement("data");
            xElement.Add(new XAttribute("name", key));
            xElement.Add(new XAttribute(XNamespace.Xml + "space", "preserve"));
            xElement.Add(new XElement("value", literal.Token.ValueText));
            var xDocument = XDocument.Load(resx.FullName);
            xDocument.Root.Add(xElement);
            using (var stream = File.OpenWrite(resx.FullName))
            {
                xDocument.Save(stream);
            }

            var designer = new FileInfo(resx.FullName.Replace("Resources.resx", "Resources.Designer.cs"));
            if (await document.GetSyntaxRootAsync(cancellationToken) is SyntaxNode root &&
                document.Project.Documents.TrySingle(x => x.FilePath == designer.FullName, out var designerDoc))
            {
                // Adding a temp key so that we don't have a build error until next gen.
                // internal static string Key => ResourceManager.GetString("Key", resourceCulture);
                var text = await designerDoc.GetTextAsync(cancellationToken);
                if (text.Lines.TryElementAt(text.Lines.Count - 3, out var line))
                {
                    text = text.WithChanges(
                        new TextChange(
                            line.Span,
                            $"\r\n        internal static string {key} => ResourceManager.GetString(\"{key}\", resourceCulture);\r\n{line}"));
                }

                return document.Project.Solution.WithDocumentSyntaxRoot(
                        document.Id,
                        root.ReplaceNode(literal, expression))
                    .WithDocumentText(
                        designerDoc.Id,
                        text);
            }

            return document.Project.Solution;
        }

        private static bool TryGetResx(INamedTypeSymbol resourcesType, out FileInfo resx)
        {
            if (resourcesType.DeclaringSyntaxReferences.TrySingle(out var reference) &&
                reference.SyntaxTree?.FilePath is string resourcesFileName &&
                resourcesFileName.Replace("Resources.Designer.cs", "Resources.resx") is string resxName &&
                File.Exists(resxName))
            {
                resx = new FileInfo(resxName);
                return true;
            }

            resx = null;
            return false;
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

            if (key.Length > 50)
            {
                key = key.Substring(50);
            }

            return SyntaxFacts.IsValidIdentifier(key);
        }

        private static bool TryFindCustomTranslate(INamedTypeSymbol resources, out IMethodSymbol customTranslate)
        {
            if (Translate.TryFindCustomToString(resources, out customTranslate) &&
                customTranslate.Parameters.TryFirst(out var parameter) &&
                parameter.Type == KnownSymbol.String)
            {
                return customTranslate.Parameters.Length == 1 ||
                       (customTranslate.Parameters.TryElementAt(1, out parameter) &&
                        parameter.IsOptional);
            }

            return false;
        }

        private class PreviewCodeAction : CodeAction
        {
            private readonly CodeAction preview;
            private readonly CodeAction change;

            public PreviewCodeAction(
                string title,
                Func<CancellationToken, Task<Document>> preview,
                Func<CancellationToken, Task<Solution>> change,
                string equivalenceKey = null)
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
