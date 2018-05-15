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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MakeInterpolatedFix))]
    internal class MakeInterpolatedFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            GULOC06UseInterpolation.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => null;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan) is LiteralExpressionSyntax literal)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Prefix with $",
                            _ => Task.FromResult(
                                context.Document.WithSyntaxRoot(
                                    syntaxRoot.ReplaceNode(
                                        literal,
                                        CreateInterpolatedStringExpression(literal)))),
                            "Prefix with $"),
                        diagnostic);
                }
            }
        }

        private static InterpolatedStringExpressionSyntax CreateInterpolatedStringExpression(LiteralExpressionSyntax literal)
        {
            return SyntaxFactory.InterpolatedStringExpression(
                SyntaxFactory.Token(
                    literal.Token.Text.StartsWith("@")
                        ? SyntaxKind.InterpolatedVerbatimStringStartToken
                        : SyntaxKind.InterpolatedStringStartToken),
                SyntaxFactory.SingletonList<InterpolatedStringContentSyntax>(CreateInterpolatedStringText(literal)),
                SyntaxFactory.Token(SyntaxKind.InterpolatedStringEndToken));
        }

        private static InterpolatedStringTextSyntax CreateInterpolatedStringText(LiteralExpressionSyntax literal)
        {
            return SyntaxFactory.InterpolatedStringText(
                SyntaxFactory.Token(
                    leading: default(SyntaxTriviaList),
                    kind: SyntaxKind.InterpolatedStringTextToken,
                    text: literal.Token.ValueText,
                    valueText: string.Empty,
                    trailing: default(SyntaxTriviaList)));
        }
    }
}
