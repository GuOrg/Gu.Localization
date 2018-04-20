namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TranslateResourceFix))]
    internal class TranslateResourceFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            GULOC02UseNameOf.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => null;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan) is MemberAccessExpressionSyntax memberAccess)
                {
                    if (diagnostic.Properties.TryGetValue(nameof(Translate), out var call))
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                $"{call}",
                                _ => Task.FromResult(
                                    context.Document.WithSyntaxRoot(
                                        syntaxRoot.ReplaceNode(
                                            memberAccess,
                                            SyntaxFactory.ParseExpression(call))))),
                            diagnostic);
                    }

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            $"Translator.Translate",
                            _ => Task.FromResult(
                                context.Document.WithSyntaxRoot(
                                    syntaxRoot.ReplaceNode(
                                        memberAccess,
                                        SyntaxFactory.ParseExpression(
                                                         $"Gu.Localization.Translator.Translate({memberAccess.Expression}.ResourceManager, nameof({memberAccess}))")
                                                     .WithSimplifiedNames())))),
                        diagnostic);
                }
            }
        }
    }
}
