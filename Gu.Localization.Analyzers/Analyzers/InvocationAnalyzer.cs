﻿namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Gu.Roslyn.AnalyzerExtensions;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class InvocationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.GULOC01KeyExists,
            Descriptors.GULOC02UseNameOf,
            Descriptors.GULOC04UseCustomTranslate);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.InvocationExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (!context.IsExcludedFromAnalysis() &&
                context.Node is InvocationExpressionSyntax { ArgumentList: { } argumentList } invocation)
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
                                    Descriptors.GULOC02UseNameOf,
                                    keyArgument.GetLocation(),
                                    ImmutableDictionary<string, string?>.Empty.Add(
                                        nameof(MemberAccessExpressionSyntax),
                                        resources.ToString())));
                        }
                        else if (keyArgument.Expression is MemberAccessExpressionSyntax)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptors.GULOC02UseNameOf, keyArgument.GetLocation()));
                        }
                    }

                    if (context.SemanticModel.GetSymbolInfo(resources).Symbol is INamedTypeSymbol resourcesType)
                    {
                        if (!KeyExists(keyArgument, resourcesType, context))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptors.GULOC01KeyExists, keyArgument.GetLocation()));
                        }

                        if (invocation.ArgumentList.Arguments.Count == 2 &&
                            !Descriptors.GULOC04UseCustomTranslate.IsSuppressed(context.SemanticModel) &&
                            TryGetCustom(target, resourcesType, out var custom))
                        {
                            var customCall = $"{custom.ContainingType.ToMinimalDisplayString(context.SemanticModel, invocation.SpanStart, SymbolDisplayFormat.MinimallyQualifiedFormat)}.{custom.Name}({keyArgument})";
                            context.ReportDiagnostic(Diagnostic.Create(Descriptors.GULOC04UseCustomTranslate, invocation.GetLocation(), ImmutableDictionary<string, string?>.Empty.Add(nameof(Translate), customCall)));
                        }
                    }
                }
                else if (argumentList.Arguments.TryFirst(out keyArgument) &&
                         (ResourceManager.IsGetObject(invocation, context, out var resourcesType, out _) ||
                          ResourceManager.IsGetString(invocation, context, out resourcesType, out _) ||
                          Translate.IsCustomTranslateMethod(invocation, context, out resourcesType, out _)))
                {
                    if (!KeyExists(keyArgument, resourcesType, context))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptors.GULOC01KeyExists, keyArgument.GetLocation()));
                    }

                    if (!IsNameOfKey(keyArgument))
                    {
                        if (keyArgument.Expression is LiteralExpressionSyntax literal &&
                            literal.IsKind(SyntaxKind.StringLiteralExpression) &&
                            resourcesType.TryFindProperty(literal.Token.ValueText, out _))
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    Descriptors.GULOC02UseNameOf,
                                    keyArgument.GetLocation(),
                                    ImmutableDictionary<string, string?>.Empty.Add(
                                        nameof(MemberAccessExpressionSyntax),
                                        resourcesType.ToMinimalDisplayString(context.SemanticModel, keyArgument.SpanStart, SymbolDisplayFormat.MinimallyQualifiedFormat))));
                        }
                        else if (Resources.IsResourceKey(keyArgument.Expression, out _))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptors.GULOC02UseNameOf, keyArgument.GetLocation()));
                        }
                    }
                }
            }
        }

        private static bool KeyExists(ArgumentSyntax keyArgument, INamedTypeSymbol resourcesType, SyntaxNodeAnalysisContext context)
        {
            if (TryGetStringValue(keyArgument, out var key) &&
                key is { })
            {
                return resourcesType.GetMembers(key).Any();
            }

            if (keyArgument.Expression is InvocationExpressionSyntax { ArgumentList.Arguments.Count: 0, Expression: MemberAccessExpressionSyntax memberAccess } &&
                context.SemanticModel.TryGetType(memberAccess.Expression, context.CancellationToken, out var candidateType) &&
                candidateType.TypeKind == TypeKind.Enum &&
                candidateType is INamedTypeSymbol enumType)
            {
                foreach (var name in enumType.MemberNames)
                {
                    if (!resourcesType.GetMembers(name).Any())
                    {
                        return false;
                    }
                }

                return true;
            }

            return true; // Assuming ok here.
        }

        private static bool TryGetCustom(IMethodSymbol target, INamedTypeSymbol resourcesType, [NotNullWhen(true)] out IMethodSymbol? custom)
        {
            if (target == KnownSymbol.Translator.Translate &&
                Translate.TryFindCustomToString(resourcesType, out custom))
            {
                return CanFix(custom);
            }

            if (target == KnownSymbol.Translation.GetOrCreate &&
                Translate.TryFindCustomToTranslation(resourcesType, out custom))
            {
                return CanFix(custom);
            }

            custom = null;
            return false;

            static bool CanFix(IMethodSymbol candidate)
            {
                if (candidate.Parameters.TryFirst(out var parameter) &&
                    parameter.Type.SpecialType == SpecialType.System_String)
                {
                    return candidate.Parameters.Length == 0 ||
                           (candidate.Parameters.TryElementAt(1, out parameter) &&
                           parameter is { IsOptional: true });
                }

                return false;
            }
        }

        private static bool IsNameOfKey(ArgumentSyntax argument)
        {
            return argument.Expression is InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" }, ArgumentList.Arguments: { Count: 1 } arguments } &&
                   arguments[0] is { Expression: MemberAccessExpressionSyntax _ };
        }

        private static bool TryGetStringValue(ArgumentSyntax argument, out string? result)
        {
            result = null;
            if (argument?.Expression is null)
            {
                return false;
            }

            switch (argument.Expression)
            {
                case LiteralExpressionSyntax literal
                    when literal.IsKind(SyntaxKind.StringLiteralExpression):
                    result = literal.Token.ValueText;
                    return true;
                case LiteralExpressionSyntax literal
                    when literal.IsKind(SyntaxKind.NullLiteralExpression):
                    result = null;
                    return true;
                case InvocationExpressionSyntax invocation
                    when invocation.IsNameOf():
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

                case MemberAccessExpressionSyntax memberAccess
                    when memberAccess.IsResources():
                    result = string.Empty;
                    return true;
            }

            return false;
        }
    }
}
