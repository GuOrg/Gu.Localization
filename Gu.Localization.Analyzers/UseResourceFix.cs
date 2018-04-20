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
                                                key,
                                                resourcesType,
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
                                            key,
                                            resourcesType,
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
                                            key,
                                            resourcesType,
                                            cancellationToken)),
                                    diagnostic);
                            }
                        }
                    }
                }
            }
        }

        private static async Task<Solution> AddResourceAndReplaceAsync(Document document, LiteralExpressionSyntax literal, ExpressionSyntax expression, string key, INamedTypeSymbol resourcesType, CancellationToken cancellationToken)
        {
            if (await document.GetSyntaxRootAsync(cancellationToken) is SyntaxNode root &&
                resourcesType.DeclaringSyntaxReferences.TrySingle(out var declaration) &&
                document.Project.Documents.TrySingle(x => x.FilePath == declaration.SyntaxTree.FilePath, out var designerDoc) &&
                TryGetResx(resourcesType, out var resx))
            {
                var designerText = await designerDoc.GetTextAsync(cancellationToken);
                var resxId = DocumentId.CreateNewId(document.Project.Id);
                var resxText = SourceText.From(File.ReadAllText(resx.FullName));
                return document.Project.Solution.WithDocumentSyntaxRoot(
                        document.Id,
                        root.ReplaceNode(literal, expression))
                    .WithDocumentText(
                        designerDoc.Id,
                        designerText.WithChanges(AddProperty(designerText)))
                    .AddAdditionalDocument(
                        resxId,
                        "Designer.resx",
                        resxText,
                        filePath: resx.FullName)
                    .WithAdditionalDocumentText(
                        resxId,
                        resxText.WithChanges(AddElement(resxText)));
            }

            return document.Project.Solution;

            IEnumerable<TextChange> AddProperty(SourceText designerText)
            {
                // Adding a temp key so that we don't have a build error until next gen.
                // internal static string Key => ResourceManager.GetString("Key", resourceCulture);
                if (designerText.Lines.TryElementAt(designerText.Lines.Count - 3, out var line))
                {
                    return new[]
                    {
                        new TextChange(
                            line.Span,
                            $"\r\n        internal static string {key} => ResourceManager.GetString(\"{key}\", resourceCulture);\r\n{line}"),
                    };
                }

                return new TextChange[0];
            }

            IEnumerable<TextChange> AddElement(SourceText resxText)
            {
                // <data name="Key" xml:space="preserve">
                //   <value>Value</value>
                // </data>
                if (resxText.Lines.TryElementAt(resxText.Lines.Count - 1, out var line))
                {
                    return new[]
                    {
                        new TextChange(
                            line.Span,
#pragma warning disable SA1118 // Parameter must not span multiple lines
                            $"  <data name=\"{key}\" xml:space=\"preserve\">\r\n" +
                            $"    <value>{literal.Token.ValueText}</value>\r\n" +
                            $"  </data>\r\n" +
                            $"{line}"),
#pragma warning restore SA1118 // Parameter must not span multiple lines
                    };
                }

                return new TextChange[0];
            }
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
