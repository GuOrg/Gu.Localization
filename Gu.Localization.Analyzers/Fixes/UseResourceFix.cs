namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseResourceFix))]
    public class UseResourceFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GULOC03UseResource.Id);

        public override FixAllProvider? GetFixAllProvider() => null;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) is { } syntaxRoot &&
                await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false) is { } semanticModel)
            {
                foreach (var diagnostic in context.Diagnostics)
                {
                    if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is LiteralExpressionSyntax literal &&
                        Resources.TryGetKey(literal.Token.ValueText, out var key))
                    {
                        foreach (var resourcesType in semanticModel.LookupResourceTypes(diagnostic.Location.SourceSpan.Start, name: "Resources"))
                        {
                            if (resourcesType.TryFindProperty(key, out _))
                            {
                                var memberAccess = resourcesType.ToMinimalDisplayString(semanticModel, literal.SpanStart, SymbolDisplayFormat.MinimallyQualifiedFormat);
                                if (semanticModel.ReferencesGuLocalization())
                                {
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
                                                                .WithSimplifiedNames()))),
                                                $"Use existing {memberAccess}.{key} in {translateKey}({memberAccess}.{key})"),
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
                                                            .WithSimplifiedNames()))),
                                            $"Use existing {memberAccess}.{key} in Translator.Translate"),
                                        diagnostic);
                                }

                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        $"Use existing {memberAccess}.{key}.",
                                        _ => Task.FromResult(
                                            context.Document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    literal,
                                                    SyntaxFactory.ParseExpression($"{memberAccess}.{key}")
                                                                 .WithSimplifiedNames()))),
                                        $"Use existing {memberAccess}.{key}."),
                                    diagnostic);
                            }
                            else if (ResxFile.TryGetDefault(resourcesType, out _))
                            {
                                var memberAccess = resourcesType.ToMinimalDisplayString(semanticModel, literal.SpanStart, SymbolDisplayFormat.MinimallyQualifiedFormat);
                                if (semanticModel.ReferencesGuLocalization())
                                {
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
                                                            SyntaxFactory.ParseExpression($"{translateKey}(nameof({memberAccess}.{key}))")
                                                                         .WithSimplifiedNames()))),
                                                cancellationToken => AddAndUseResourceAsync(
                                                    context.Document,
                                                    literal,
                                                    SyntaxFactory.ParseExpression($"{translateKey}(nameof({memberAccess}.{key}))")
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
                                                        SyntaxFactory.ParseExpression($"Gu.Localization.Translator.Translate({memberAccess}.ResourceManager, nameof({memberAccess}.{key}))")
                                                                     .WithSimplifiedNames()))),
                                            cancellationToken => AddAndUseResourceAsync(
                                                context.Document,
                                                literal,
                                                SyntaxFactory.ParseExpression(
                                                        $"Gu.Localization.Translator.Translate({memberAccess}.ResourceManager, nameof({memberAccess}.{key}))")
                                                    .WithSimplifiedNames(),
                                                key,
                                                resourcesType,
                                                cancellationToken)),
                                        diagnostic);
                                }

                                context.RegisterCodeFix(
                                    new PreviewCodeAction(
                                        $"Move to {memberAccess}.{key}.",
                                        _ => Task.FromResult(
                                            context.Document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    literal,
                                                    SyntaxFactory.ParseExpression($"{memberAccess}.{key}")
                                                                 .WithSimplifiedNames()))),
                                        cancellationToken => AddAndUseResourceAsync(
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

        private static async Task<Solution> AddAndUseResourceAsync(Document document, LiteralExpressionSyntax literal, ExpressionSyntax expression, string key, INamedTypeSymbol resourcesType, CancellationToken cancellationToken)
        {
            if (await document.GetSyntaxRootAsync(cancellationToken) is SyntaxNode root &&
                resourcesType.DeclaringSyntaxReferences.TrySingle(out var declaration) &&
                document.Project.Documents.TrySingle(x => x.FilePath == declaration.SyntaxTree.FilePath, out var designerDoc) &&
                await designerDoc.GetSyntaxRootAsync(cancellationToken) is SyntaxNode designerRoot &&
                ResxFile.TryGetDefault(resourcesType, out var resx))
            {
                resx.Add(key, literal.Token.ValueText);
                return document.Project.Solution.WithDocumentSyntaxRoot(
                        document.Id,
                        root.ReplaceNode(literal, expression))
                    .WithDocumentSyntaxRoot(
                        designerDoc.Id,
                        AddProperty());
            }

            return document.Project.Solution;

            SyntaxNode AddProperty()
            {
                // Adding a temp key so that we don't have a build error until next gen.
                // public static string Key => ResourceManager.GetString("Key", resourceCulture);
                if (designerRoot.DescendantNodes().TryLastOfType(out PropertyDeclarationSyntax property))
                {
                    return designerRoot.InsertNodesAfter(
                        property,
#pragma warning disable SA1118 // Parameter must not span multiple lines
                        new[]
                        {
                            SyntaxFactory.PropertyDeclaration(
                                default,
                                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                                SyntaxFactory.ParseTypeName("string"),
                                default,
                                SyntaxFactory.ParseToken(key),
                                default,
                                SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ParseExpression($"ResourceManager.GetString(\"{key}\", resourceCulture)")),
                                default,
                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        });
#pragma warning restore SA1118 // Parameter must not span multiple lines
                }

                return designerRoot;
            }
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
    }
}
