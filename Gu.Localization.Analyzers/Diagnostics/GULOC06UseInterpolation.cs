namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class GULOC06UseInterpolation
    {
        public const string DiagnosticId = "GULOC06";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Use interpolation.",
            messageFormat: "Use interpolation.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Use interpolation.");
    }
}
