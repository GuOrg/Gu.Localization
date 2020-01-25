namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class Descriptors
    {
        internal static readonly DiagnosticDescriptor GULOC01KeyExists = new DiagnosticDescriptor(
            id: "GULOC01",
            title: "Key does not exist.",
            messageFormat: "Key does not exist.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Key does not exist.");

        internal static readonly DiagnosticDescriptor GULOC02UseNameOf = new DiagnosticDescriptor(
            id: "GULOC02",
            title: "Use nameof(key).",
            messageFormat: "Use nameof(key).",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Use nameof(key).");

        internal static readonly DiagnosticDescriptor GULOC03UseResource = new DiagnosticDescriptor(
            id: "GULOC03",
            title: "Use resource.",
            messageFormat: "Use resource.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Use resource.");

        internal static readonly DiagnosticDescriptor GULOC04UseCustomTranslate = new DiagnosticDescriptor(
            id: "GULOC04",
            title: "Use custom translate.",
            messageFormat: "Use custom translate.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false,
            description: "Use custom translate.");

        internal static readonly DiagnosticDescriptor GULOC05TranslateUseResource = new DiagnosticDescriptor(
            id: "GULOC05",
            title: "Translate resource.",
            messageFormat: "Translate resource.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Translate resource.");

        internal static readonly DiagnosticDescriptor GULOC06UseInterpolation = new DiagnosticDescriptor(
            id: "GULOC06",
            title: "Use interpolation.",
            messageFormat: "Use interpolation.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Use interpolation.");

        internal static readonly DiagnosticDescriptor GULOC07KeyDoesNotMatch = new DiagnosticDescriptor(
            id: "GULOC07",
            title: "Key does not match.",
            messageFormat: "Expected the name to be {0}",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "The resource key does not match the resource text.");

        internal static readonly DiagnosticDescriptor GULOC08DuplicateNeutral = new DiagnosticDescriptor(
            id: "GULOC08",
            title: "More than one resource has the same neutral string.",
            messageFormat: "The duplicated neutral string is {0}",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "More than one resource has the same neutral string.");

        internal static readonly DiagnosticDescriptor GULOC09Duplicate = new DiagnosticDescriptor(
            id: "GULOC09",
            title: "The resource is a duplicate in all cultures.",
            messageFormat: "The duplicated neutral string is {0}",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The resource is a duplicate in all cultures.");

        internal static readonly DiagnosticDescriptor GULOC10MissingTranslation = new DiagnosticDescriptor(
            id: "GULOC10",
            title: "Missing translation.",
            messageFormat: "The resource does not have translation to '{0}', the neutral string is '{1}'",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "Missing translation.");

        private static DiagnosticDescriptor Create(
            string id,
            string title,
            string messageFormat,
            string category,
            DiagnosticSeverity defaultSeverity,
            bool isEnabledByDefault,
            string description)
        {
            return new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: messageFormat,
                category: category,
                defaultSeverity: defaultSeverity,
                isEnabledByDefault: isEnabledByDefault,
                description: description,
                helpLinkUri: null,
                customTags: null);
        }
    }
}
