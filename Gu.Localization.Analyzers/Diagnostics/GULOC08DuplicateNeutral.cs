namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class GULOC08DuplicateNeutral
    {
        public const string DiagnosticId = "GULOC08";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "More than one resource has the same neutral string.",
            messageFormat: "The neutral string is {0}",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "More than one resource has the same neutral string.");
    }
}