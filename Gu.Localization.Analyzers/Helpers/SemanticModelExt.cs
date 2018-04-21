namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class SemanticModelExt
    {
        internal static bool ReferencesGuLocalization(this SemanticModel semanticModel)
        {
            return semanticModel.Compilation.ReferencedAssemblyNames.TryFirst(x => x.Name == "Gu.Localization", out _);
        }
    }
}
