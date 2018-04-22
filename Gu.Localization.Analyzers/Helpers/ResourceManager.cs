namespace Gu.Localization.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class ResourceManager
    {
        internal static bool IsGetObject(this InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, out INamedTypeSymbol resourcesType, out IMethodSymbol method)
        {
            method = null;
            resourcesType = null;
            if (invocation.Expression is InstanceExpressionSyntax ||
                invocation.Expression == null ||
                invocation.ArgumentList == null)
            {
                return false;
            }

            if (invocation.ArgumentList.Arguments.TryFirst(out _) &&
                invocation.TryGetMethodName(out var name) &&
                name == "GetObject" &&
                invocation.Expression is MemberAccessExpressionSyntax getObject &&
                getObject.Expression is MemberAccessExpressionSyntax resourceManager &&
                Resources.IsResourceManager(resourceManager, out var resources) &&
                context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol target &&
                target == KnownSymbol.ResourceManager.GetObject)
            {
                resourcesType = context.SemanticModel.GetSymbolInfo(resources, context.CancellationToken).Symbol as INamedTypeSymbol;
                method = target;
            }

            return method != null && resourcesType != null;
        }
    }
}
