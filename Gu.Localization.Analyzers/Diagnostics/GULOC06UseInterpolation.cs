namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GULOC06UseInterpolation
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: "GULOC06",
            title: "Use interpolation.",
            messageFormat: "Use interpolation.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Use interpolation.");
    }
}
