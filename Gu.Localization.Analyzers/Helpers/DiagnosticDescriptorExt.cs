namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class DiagnosticDescriptorExt
    {
        internal static bool IsSuppressed(this DiagnosticDescriptor descriptor, SemanticModel semanticModel)
        {
            if (semanticModel.Compilation.Options.SpecificDiagnosticOptions.TryGetValue(descriptor.Id, out var report))
            {
                return report == ReportDiagnostic.Suppress;
            }

            return !descriptor.IsEnabledByDefault;
        }
    }
}