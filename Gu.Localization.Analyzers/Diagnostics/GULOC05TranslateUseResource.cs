namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GULOC05TranslateUseResource
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: "GULOC05",
            title: "Translate resource.",
            messageFormat: "Translate resource.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Translate resource.");
    }
}
