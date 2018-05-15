namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class GULOC07KeyDoesNotMatch
    {
        public const string DiagnosticId = "GULOC07";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Key does not match.",
            messageFormat: "Expected {0}",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "The resource key does not match the resource text.");
    }
}