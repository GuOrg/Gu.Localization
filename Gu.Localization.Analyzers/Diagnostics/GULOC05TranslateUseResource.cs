namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class GULOC05TranslateUseResource
    {
        public const string DiagnosticId = "GULOC05";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Translate resource.",
            messageFormat: "Translate resource.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Translate resource.");
    }
}
