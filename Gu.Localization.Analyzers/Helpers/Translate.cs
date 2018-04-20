namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class Translate
    {
        internal static bool IsCustonTranslateMethod(this InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, out ITypeSymbol resourcesType, out IMethodSymbol method)
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
                context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol target &&
                target.IsStatic &&
                (target.ReturnType == KnownSymbol.String ||
                 target.ReturnType == KnownSymbol.ITranslation) &&
                target.Parameters.TryFirst(out var parameter) &&
                parameter.Type == KnownSymbol.String &&
                target.ContainingNamespace.GetTypeMembers("Resources").TryFirst(out resourcesType))
            {
                method = target;
            }

            return method != null;
        }
    }
}
