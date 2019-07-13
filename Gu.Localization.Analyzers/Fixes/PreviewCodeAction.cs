namespace Gu.Localization.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;

    internal class PreviewCodeAction : CodeAction
    {
        private readonly CodeAction preview;
        private readonly CodeAction change;

        internal PreviewCodeAction(
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

        internal PreviewCodeAction(
            string title,
            Func<CancellationToken, Task<Solution>> preview,
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
