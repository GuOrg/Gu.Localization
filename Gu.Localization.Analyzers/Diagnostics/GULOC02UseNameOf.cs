namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class GULOC02UseNameOf
    {
        public const string DiagnosticId = "GULOC02";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Use nameof(key).",
            messageFormat: "Use nameof(key).",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Use nameof(key).");
    }
}
