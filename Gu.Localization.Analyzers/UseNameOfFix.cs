namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Gu.Localization.Analyzers.FixAll;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseNameOfFix))]
    internal class UseNameOfFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            UseNameOfAnalyzer.DiagnosticId);

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                                   .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                var argument = syntaxRoot.FindNode(diagnostic.Location.SourceSpan)
                                     .FirstAncestorOrSelf<ArgumentSyntax>();
                switch (argument.Expression)
                {
                    case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression) &&
                        diagnostic.Properties.TryGetValue(nameof(MemberAccessExpressionSyntax), out var member):
                        context.RegisterCodeFix(
                            "Use nameof.",
                            (editor, _) => editor.ReplaceNode(
                                literal,
                                (x, g) => SyntaxFactory.ParseExpression($"nameof({member}.{literal.Token.ValueText})")),
                            "Use nameof.",
                            diagnostic);
                        break;
                    case MemberAccessExpressionSyntax memberAccess:
                        context.RegisterCodeFix(
                            "Use nameof.",
                            (editor, _) => editor.ReplaceNode(
                                memberAccess,
                                (x, g) => SyntaxFactory.ParseExpression($"nameof({memberAccess})")),
                            "Use nameof.",
                            diagnostic);
                        break;
                }
            }
        }
    }
}
