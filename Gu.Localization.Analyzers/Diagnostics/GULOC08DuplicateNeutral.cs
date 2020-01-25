namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GULOC08DuplicateNeutral
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: "GULOC08",
            title: "More than one resource has the same neutral string.",
            messageFormat: "The duplicated neutral string is {0}",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "More than one resource has the same neutral string.");
    }
}
