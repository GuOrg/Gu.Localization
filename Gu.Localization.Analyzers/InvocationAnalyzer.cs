namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class InvocationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            KeyExistsAnalyzer.Descriptor,
            UseNameOfAnalyzer.Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Handle, SyntaxKind.InvocationExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation &&
                invocation.ArgumentList is ArgumentListSyntax argumentList)
            {
                if (argumentList.Arguments.TryFirst(out var resourceManagerArgument) &&
                    Resources.IsResourceManager(resourceManagerArgument.Expression, out var resources) &&
                    argumentList.Arguments.TryElementAt(1, out var keyArgument) &&
                    context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol target &&
                    (target == KnownSymbol.Translator.Translate ||
                     target == KnownSymbol.Translation.GetOrCreate))
                {
                    if (!IsNameOfKey(keyArgument))
                    {
                        if (keyArgument.Expression is LiteralExpressionSyntax)
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    UseNameOfAnalyzer.Descriptor,
                                    keyArgument.GetLocation(),
                                    ImmutableDictionary<string, string>.Empty.Add(
                                        nameof(MemberAccessExpressionSyntax),
                                        resources.ToString())));
                        }
                        else if (keyArgument.Expression is MemberAccessExpressionSyntax)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(UseNameOfAnalyzer.Descriptor, keyArgument.GetLocation()));
                        }
                    }

                    if (TryGetStringValue(keyArgument, out var key) &&
                        context.SemanticModel.GetSymbolInfo(resources).Symbol is ITypeSymbol resourcesType &&
                        !resourcesType.GetMembers(key).Any())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(KeyExistsAnalyzer.Descriptor, keyArgument.GetLocation()));
                    }
                }
                else if (argumentList.Arguments.TryFirst(out keyArgument) &&
                        context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol translateMethod &&
                        translateMethod.IsStatic &&
                        (translateMethod.ReturnType == KnownSymbol.String ||
                         translateMethod.ReturnType == KnownSymbol.ITranslation) &&
                        translateMethod.ContainingType.Name == "Translate" &&
                        translateMethod.Parameters.TryFirst(out var parameter) &&
                        parameter.Type == KnownSymbol.String &&
                        translateMethod.ContainingNamespace.GetTypeMembers("Resources").FirstOrDefault() is ITypeSymbol resourcesType)
                {
                    if (!IsNameOfKey(keyArgument))
                    {
                        if (keyArgument.Expression is LiteralExpressionSyntax)
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    UseNameOfAnalyzer.Descriptor,
                                    keyArgument.GetLocation(),
                                    ImmutableDictionary<string, string>.Empty.Add(
                                        nameof(MemberAccessExpressionSyntax),
                                        resourcesType.ToMinimalDisplayString(context.SemanticModel, keyArgument.SpanStart, SymbolDisplayFormat.MinimallyQualifiedFormat))));
                        }
                        else if (keyArgument.Expression is MemberAccessExpressionSyntax)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(UseNameOfAnalyzer.Descriptor, keyArgument.GetLocation()));
                        }
                    }

                    if (TryGetStringValue(keyArgument, out var key) &&
                        !resourcesType.GetMembers(key).Any())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(KeyExistsAnalyzer.Descriptor, keyArgument.GetLocation()));
                    }
                }
            }
        }

        private static bool IsNameOfKey(ArgumentSyntax argument)
        {
            return argument.Expression is InvocationExpressionSyntax invocation &&
                   invocation.IsNameOf() &&
                   invocation.ArgumentList is ArgumentListSyntax argumentList &&
                   argumentList.Arguments.TrySingle(out var keyArg) &&
                   keyArg.Expression is MemberAccessExpressionSyntax;
        }

        private static bool TryGetStringValue(ArgumentSyntax argument, out string result)
        {
            result = null;
            if (argument?.Expression == null)
            {
                return false;
            }

            switch (argument.Expression)
            {
                case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                    result = literal.Token.ValueText;
                    return true;
                case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.NullLiteralExpression):
                    result = null;
                    return true;
                case InvocationExpressionSyntax invocation when invocation.IsNameOf():
                    if (invocation.ArgumentList != null &&
                        invocation.ArgumentList.Arguments.TrySingle(out var nameofArg))
                    {
                        switch (nameofArg.Expression)
                        {
                            case IdentifierNameSyntax identifierName:
                                result = identifierName.Identifier.ValueText;
                                return true;
                            case MemberAccessExpressionSyntax memberAccess:
                                result = memberAccess.Name.Identifier.ValueText;
                                return true;
                        }
                    }

                    break;

                case MemberAccessExpressionSyntax memberAccess when memberAccess.IsResources():
                    result = string.Empty;
                    return true;
            }

            return false;
        }
    }
}
