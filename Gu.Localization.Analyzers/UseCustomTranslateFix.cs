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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseCustomTranslateFix))]
    internal class UseCustomTranslateFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            GULOC04UseCustomTranslate.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => null;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (await context.Document.GetSyntaxRootAsync(context.CancellationToken) is SyntaxNode syntaxRoot)
            {
                foreach (var diagnostic in context.Diagnostics)
                {
                    if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan) is InvocationExpressionSyntax invocation &&
                        diagnostic.Properties.TryGetValue(nameof(Translate), out var call))
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                $"Use {call}",
                                _ => Task.FromResult(
                                    context.Document.WithSyntaxRoot(
                                        syntaxRoot.ReplaceNode(
                                            invocation,
                                            SyntaxFactory.ParseExpression(call))))),
                            diagnostic);
                    }
                }
            }
        }
    }
}
