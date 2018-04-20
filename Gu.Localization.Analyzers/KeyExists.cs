namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class KeyExists
    {
        public const string DiagnosticId = "GULOC01";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Key does not exist.",
            messageFormat: "Key does not exist.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Key does not exist.");
    }
}
