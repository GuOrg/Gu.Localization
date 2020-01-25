namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GULOC09Duplicate
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: "GULOC09",
            title: "The resource is a duplicate in all cultures.",
            messageFormat: "The duplicated neutral string is {0}",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The resource is a duplicate in all cultures.");
    }
}
