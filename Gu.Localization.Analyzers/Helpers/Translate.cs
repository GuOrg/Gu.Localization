namespace Gu.Localization.Analyzers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class Translate
    {
        internal static bool IsCustomTranslateMethod(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, [NotNullWhen(true)] out INamedTypeSymbol? resourcesType, [NotNullWhen(true)] out IMethodSymbol? method)
        {
            method = null;
            resourcesType = null;
            if (invocation.Expression is InstanceExpressionSyntax ||
                invocation.Expression is null ||
                invocation.ArgumentList is null ||
                !context.SemanticModel.ReferencesGuLocalization())
            {
                return false;
            }

            if (invocation.ArgumentList.Arguments.TryFirst(out _) &&
                context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol target &&
                IsCustomTranslateMethod(target) &&
                target.ContainingNamespace.GetTypeMembers("Resources").TryFirst(out resourcesType))
            {
                method = target;
            }

            return method != null;

            bool IsCustomTranslateMethod(IMethodSymbol candidate)
            {
                return candidate.IsStatic &&
                       (candidate.ReturnType == KnownSymbol.String ||
                        candidate.ReturnType == KnownSymbol.ITranslation) &&
                       candidate.Parameters.TryFirst(out var parameter) &&
                       parameter.Type == KnownSymbol.String &&
                       candidate.ContainingNamespace.GetTypeMembers("Resources").Any();
            }
        }

        internal static bool TryFindCustomToString(INamedTypeSymbol resourcesType, [NotNullWhen(true)] out IMethodSymbol? method)
        {
            method = null;
            if (resourcesType is null)
            {
                return false;
            }

            foreach (var type in resourcesType.ContainingNamespace.GetTypeMembers("Translate"))
            {
                if (type.TryFindSingleMethod(symbol => IsCustomTranslateMethod(symbol), out method))
                {
                    return true;
                }
            }

            return false;

            bool IsCustomTranslateMethod(IMethodSymbol candidate)
            {
                return candidate.IsStatic &&
                       candidate.ReturnType == KnownSymbol.String &&
                       candidate.Parameters.TryFirst(out var parameter) &&
                       parameter.Type == KnownSymbol.String &&
                       candidate.ContainingNamespace.GetTypeMembers("Resources").Any();
            }
        }

        internal static bool TryFindCustomToTranslation(INamedTypeSymbol resourcesType, [NotNullWhen(true)] out IMethodSymbol? method)
        {
            method = null;
            if (resourcesType is null)
            {
                return false;
            }

            foreach (var type in resourcesType.ContainingNamespace.GetTypeMembers("Translate"))
            {
                if (type.TryFindSingleMethod(symbol => IsCustomTranslateMethod(symbol), out method))
                {
                    return true;
                }
            }

            return false;

            bool IsCustomTranslateMethod(IMethodSymbol candidate)
            {
                return candidate.IsStatic &&
                       candidate.ReturnType == KnownSymbol.ITranslation &&
                       candidate.Parameters.TryFirst(out var parameter) &&
                       parameter.Type == KnownSymbol.String &&
                       candidate.ContainingNamespace.GetTypeMembers("Resources").Any();
            }
        }
    }
}
