namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class UseCustomTranslateInfo
    {
        public const string DiagnosticId = "GULOC04";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Use custom translate.",
            messageFormat: "Use custom translate.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false,
            description: "Use custom translate.");
    }
}
