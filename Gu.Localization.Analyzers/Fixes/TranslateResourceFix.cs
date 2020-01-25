namespace Gu.Localization.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TranslateResourceFix))]
    internal class TranslateResourceFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GULOC04UseCustomTranslate.Id,
            Descriptors.GULOC05TranslateUseResource.Id);

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan) is MemberAccessExpressionSyntax memberAccess)
                {
                    if (diagnostic.Id == "GULOC04" &&
                        diagnostic.Properties.TryGetValue(nameof(Translate), out var call))
                    {
                        context.RegisterCodeFix(
                            $"{call}",
                            (editor, _) => editor.ReplaceNode(memberAccess, SyntaxFactory.ParseExpression(call)),
                            $"{call}",
                            diagnostic);
                    }

                    context.RegisterCodeFix(
                        "Gu.Localization.Translator.Translate",
                        (editor, _) => editor.ReplaceNode(
                            memberAccess,
                            SyntaxFactory.ParseExpression($"Gu.Localization.Translator.Translate({memberAccess.Expression}.ResourceManager, nameof({memberAccess}))")
                                         .WithSimplifiedNames()),
                        "Gu.Localization.Translator.Translate",
                        diagnostic);

                    if (!UsingDirectiveWalker.Contains(syntaxRoot, "Gu.Localization"))
                    {
                        context.RegisterCodeFix(
                            "Add using and call Translator.Translate",
                            (editor, _) => editor.AddUsing(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Gu.Localization")))
                                                 .ReplaceNode(
                                                     memberAccess,
                                                     SyntaxFactory.ParseExpression(
                                                                      $"Translator.Translate({memberAccess.Expression}.ResourceManager, nameof({memberAccess}))")
                                                                  .WithSimplifiedNames()),
                            "Add using and call Translator.Translate",
                            diagnostic);
                    }
                }
            }
        }

        private sealed class UsingDirectiveWalker : PooledWalker<UsingDirectiveWalker>
        {
            private readonly List<UsingDirectiveSyntax> usingDirectives = new List<UsingDirectiveSyntax>();

            public override void VisitUsingDirective(UsingDirectiveSyntax node)
            {
                this.usingDirectives.Add(node);
                base.VisitUsingDirective(node);
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                // Stop walking here
            }

            public override void VisitStructDeclaration(StructDeclarationSyntax node)
            {
                // Stop walking here
            }

            internal static bool Contains(SyntaxNode syntaxRoot, string name)
            {
                using (var walker = BorrowAndVisit(syntaxRoot, () => new UsingDirectiveWalker()))
                {
                    foreach (var directive in walker.usingDirectives)
                    {
                        if (directive.Name.ToString() == name)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            protected override void Clear()
            {
                this.usingDirectives.Clear();
            }
        }
    }
}
