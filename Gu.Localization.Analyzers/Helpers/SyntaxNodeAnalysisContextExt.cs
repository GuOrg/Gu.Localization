namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class SyntaxNodeAnalysisContextExt
    {
        internal static bool IsExcludedFromAnalysis(this SyntaxNodeAnalysisContext context)
        {
            if (context.Node == null ||
                context.Node.IsMissing ||
                context.SemanticModel == null)
            {
                return true;
            }

            return context.SemanticModel.SyntaxTree.FilePath.EndsWith(".g.cs");
        }
    }
}