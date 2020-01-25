namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class Descriptors
    {
        private static DiagnosticDescriptor Create(
            string id,
            string title,
            string messageFormat,
            string category,
            DiagnosticSeverity defaultSeverity,
            bool isEnabledByDefault,
            string description,
            params string[] customTags)
        {
            return new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: messageFormat,
                category: category,
                defaultSeverity: defaultSeverity,
                isEnabledByDefault: isEnabledByDefault,
                description: description,
                helpLinkUri: $"https://github.com/DotNetAnalyzers/PropertyChangedAnalyzers/tree/master/documentation/{id}.md",
                customTags: customTags);
        }
    }
}
