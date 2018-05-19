namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class GULOC10MissingTranslation
    {
        public const string DiagnosticId = "GULOC10";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Missing translation.",
            messageFormat: "The resource does not have translation to '{0}', the neutral string is '{1}'",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "Missing translation.");
    }
}
