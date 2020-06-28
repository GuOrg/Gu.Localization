namespace Gu.Localization.Analyzers
{
    using System.Diagnostics.CodeAnalysis;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class ResourceManager
    {
        internal static bool IsGetObject(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, [NotNullWhen(true)] out INamedTypeSymbol? resourcesType, [NotNullWhen(true)] out IMethodSymbol? method)
        {
            return IsGet(invocation, context, KnownSymbol.ResourceManager.GetObject, out resourcesType, out method);
        }

        internal static bool IsGetString(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, [NotNullWhen(true)] out INamedTypeSymbol? resourcesType, [NotNullWhen(true)] out IMethodSymbol? method)
        {
            return IsGet(invocation, context, KnownSymbol.ResourceManager.GetString, out resourcesType, out method);
        }

        private static bool IsGet(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, QualifiedMethod expected, [NotNullWhen(true)] out INamedTypeSymbol? resourcesType, [NotNullWhen(true)] out IMethodSymbol? method)
        {
            method = null;
            resourcesType = null;
            if (invocation.Expression is InstanceExpressionSyntax ||
                invocation.Expression is null ||
                invocation.ArgumentList is null)
            {
                return false;
            }

            if (invocation.ArgumentList.Arguments.TryFirst(out _) &&
                invocation.TryGetMethodName(out var name) &&
                name == expected.Name &&
                invocation.Expression is MemberAccessExpressionSyntax getX &&
                getX.Expression is MemberAccessExpressionSyntax resourceManager &&
                Resources.IsResourceManager(resourceManager, out var resources) &&
                context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol target &&
                target == expected)
            {
                resourcesType = context.SemanticModel.GetSymbolInfo(resources, context.CancellationToken).Symbol as INamedTypeSymbol;
                method = target;
            }

            return method != null && resourcesType != null;
        }
    }
}
