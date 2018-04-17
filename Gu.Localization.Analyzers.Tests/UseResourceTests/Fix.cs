namespace Gu.Localization.Analyzers.Tests.UseResourceTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Helper class for applying code fixes
    /// </summary>
    internal static class HackFix
    {
        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="codeFix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<bool> IsRegisteringFixAsync(Solution solution, CodeFixProvider codeFix, Diagnostic diagnostic)
        {
            var actions = await GetActionsAsync(solution, codeFix, diagnostic);
            return actions.Count != 0;
        }

        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="codeFix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one. If only one pass null.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAsync(Solution solution, CodeFixProvider codeFix, Diagnostic diagnostic, string fixTitle, CancellationToken cancellationToken)
        {
            var actions = await GetActionsAsync(solution, codeFix, diagnostic);
            var action = FindAction(actions, fixTitle);
            var operations = await action.GetOperationsAsync(cancellationToken)
                .ConfigureAwait(false);
            return operations.OfType<ApplyChangesOperation>()
                .Single()
                .ChangedSolution;
        }

        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAsync(CodeFixProvider codeFix, FixAllScope scope, TestDiagnosticProvider diagnosticProvider, CancellationToken cancellationToken)
        {
            var context = new FixAllContext(
                diagnosticProvider.Document,
                codeFix,
                scope,
                diagnosticProvider.EquivalenceKey,
                codeFix.FixableDiagnosticIds,
                diagnosticProvider,
                cancellationToken);
            var action = await codeFix.GetFixAllProvider().GetFixAsync(context).ConfigureAwait(false);

            var operations = await action.GetOperationsAsync(cancellationToken)
                .ConfigureAwait(false);
            return operations.OfType<ApplyChangesOperation>()
                .Single()
                .ChangedSolution;
        }

        /// <summary>
        /// Fix the solution by applying the code fix one fix at the time until it stops fixing the code.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAllFixableOneByOneAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, string fixTitle, CancellationToken cancellationToken)
        {
            var fixable = await Analyze.GetFixableDiagnosticsAsync(solution, analyzer, codeFix).ConfigureAwait(false);
            var fixedSolution = solution;
            int count;
            do
            {
                count = fixable.Count;
                if (count == 0)
                {
                    return fixedSolution;
                }

                fixedSolution = await ApplyAsync(fixedSolution, codeFix, fixable[0], fixTitle, cancellationToken).ConfigureAwait(false);
                fixable = await Analyze.GetFixableDiagnosticsAsync(fixedSolution, analyzer, codeFix).ConfigureAwait(false);
            }
            while (fixable.Count < count);
            return fixedSolution;
        }

        /// <summary>
        /// Fix the solution by applying the code fix one fix at the time until it stops fixing the code.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAllFixableScopeByScopeAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, string fixTitle, FixAllScope scope, CancellationToken cancellationToken)
        {
            var fixable = await Analyze.GetFixableDiagnosticsAsync(solution, analyzer, codeFix).ConfigureAwait(false);
            var fixedSolution = solution;
            int count;
            do
            {
                count = fixable.Count;
                if (count == 0)
                {
                    return fixedSolution;
                }

                var diagnosticProvider = await TestDiagnosticProvider.CreateAsync(fixedSolution, codeFix, fixTitle, fixable).ConfigureAwait(false);
                fixedSolution = await ApplyAsync(codeFix, scope, diagnosticProvider, cancellationToken).ConfigureAwait(false);
                fixable = await Analyze.GetFixableDiagnosticsAsync(fixedSolution, analyzer, codeFix).ConfigureAwait(false);
            }
            while (fixable.Count < count);
            return fixedSolution;
        }

        /// <summary>
        /// Get the code actions registered by <paramref name="codeFix"/> for <paramref name="solution"/>
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="codeFix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <returns>The list of registered actions.</returns>
        internal static async Task<IReadOnlyList<CodeAction>> GetActionsAsync(Solution solution, CodeFixProvider codeFix, Diagnostic diagnostic)
        {
            var document = solution.GetDocument(diagnostic.Location.SourceTree);
            var actions = new List<CodeAction>();
            var context = new CodeFixContext(
                document,
                diagnostic,
                (a, d) => actions.Add(a),
                CancellationToken.None);
            await codeFix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            return actions;
        }

        private static CodeAction FindAction(IReadOnlyList<CodeAction> actions, string fixTitle)
        {
            if (fixTitle == null)
            {
                if (actions.TrySingle(out var action))
                {
                    return action;
                }

                if (actions.Count == 0)
                {
                    throw AssertException.Create("Expected one code fix, was 0.");
                }

                throw AssertException.Create($"Expected only one code fix, found {actions.Count}:\r\n" +
                                             $"{string.Join("\r\n", actions.Select(x => x.Title))}\r\n" +
                                             "Use the overload that specifies title.");
            }
            else
            {
                if (actions.TrySingle(x => x.Title == fixTitle, out var action))
                {
                    return action;
                }

                if (actions.All(x => x.Title != fixTitle))
                {
                    var errorBuilder = new StringBuilder();
                    errorBuilder.AppendLine($"Did not find a code fix with title {fixTitle}.").AppendLine("Found:");
                    foreach (var codeAction in actions)
                    {
                        errorBuilder.AppendLine(codeAction.Title);
                    }

                    throw AssertException.Create(errorBuilder.ToString());
                }

                if (actions.Count(x => x.Title == fixTitle) == 0)
                {
                    throw AssertException.Create("Expected one code fix, was 0.");
                }

                throw AssertException.Create($"Expected only one code fix, found {actions.Count}:\r\n" +
                                             $"{string.Join("\r\n", actions.Select(x => x.Title))}\r\n" +
                                             "Use the overload that specifies title.");
            }
        }

        /// <inheritdoc />
        internal sealed class TestDiagnosticProvider : FixAllContext.DiagnosticProvider
        {
            private readonly IReadOnlyList<Diagnostic> diagnostics;

            private TestDiagnosticProvider(IReadOnlyList<Diagnostic> diagnostics, Document document, string equivalenceKey)
            {
                this.diagnostics = diagnostics;
                this.Document = document;
                this.EquivalenceKey = equivalenceKey;
            }

            /// <summary>
            /// Gets the document from the first diagnostic.
            /// </summary>
            public Document Document { get; }

            /// <summary>
            /// Gets the equivalence key for the first registered action.
            /// </summary>
            public string EquivalenceKey { get; }

            /// <inheritdoc />
            public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            {
                return Task.FromResult((IEnumerable<Diagnostic>)this.diagnostics);
            }

            /// <inheritdoc />
            public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.diagnostics.Where(i => i.Location.GetLineSpan().Path == document.Name));
            }

            /// <inheritdoc />
            public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.diagnostics.Where(i => !i.Location.IsInSource));
            }

            /// <summary>
            /// Create an instance of <see cref="TestDiagnosticProvider"/>
            /// </summary>
            /// <returns>The <see cref="TestDiagnosticProvider"/></returns>
            internal static async Task<TestDiagnosticProvider> CreateAsync(Solution solution, CodeFixProvider codeFix, string fixTitle, IReadOnlyList<Diagnostic> diagnostics)
            {
                var actions = new List<CodeAction>();
                var diagnostic = diagnostics.First();
                var context = new CodeFixContext(solution.GetDocument(diagnostic.Location.SourceTree), diagnostic, (a, d) => actions.Add(a), CancellationToken.None);
                await codeFix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
                var action = FindAction(actions, fixTitle);
                return new TestDiagnosticProvider(diagnostics, solution.GetDocument(diagnostics.First().Location.SourceTree), action.EquivalenceKey);
            }
        }
    }
}
