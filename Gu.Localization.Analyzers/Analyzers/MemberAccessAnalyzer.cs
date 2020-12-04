namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;

    using Gu.Roslyn.AnalyzerExtensions;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class MemberAccessAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.GULOC04UseCustomTranslate,
            Descriptors.GULOC05TranslateUseResource);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (!context.IsExcludedFromAnalysis() &&
                context.Node is MemberAccessExpressionSyntax memberAccess &&
                Resources.IsResourceKey(memberAccess, out var resources) &&
                !IsInNameOf(memberAccess) &&
                context.SemanticModel.GetTypeInfo(memberAccess, context.CancellationToken).Type == KnownSymbol.String &&
                context.SemanticModel.ReferencesGuLocalization() &&
                context.SemanticModel.TryGetNamedType(resources, context.CancellationToken, out var resourcesType))
            {
                if (Translate.TryFindCustomToString(resourcesType, out var custom))
                {
                    var customCall = $"{custom.ContainingType.ToMinimalDisplayString(context.SemanticModel, memberAccess.SpanStart, SymbolDisplayFormat.MinimallyQualifiedFormat)}.{custom.Name}(nameof({memberAccess}))";
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.GULOC04UseCustomTranslate, memberAccess.GetLocation(), ImmutableDictionary<string, string>.Empty.Add(nameof(Translate), customCall)));
                }
                else
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.GULOC05TranslateUseResource, context.Node.GetLocation()));
                }
            }
        }

        private static bool IsInNameOf(MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess.Parent is ArgumentSyntax { Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax invocation } argumentList } argument &&
                   invocation.IsNameOf();
        }
    }
}
