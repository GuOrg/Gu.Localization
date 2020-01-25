namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GULOC10MissingTranslation
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: "GULOC10",
            title: "Missing translation.",
            messageFormat: "The resource does not have translation to '{0}', the neutral string is '{1}'",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "Missing translation.");
    }
}
