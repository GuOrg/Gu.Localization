namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class LiteralAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            GULOC03UseResource.Descriptor,
            GULOC06UseInterpolation.Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.StringLiteralExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (!context.IsExcludedFromAnalysis() &&
                context.Node is LiteralExpressionSyntax literal &&
                !string.IsNullOrWhiteSpace(literal.Token.ValueText) &&
                !IsExcludedFile(literal.SyntaxTree.FilePath))
            {
                context.ReportDiagnostic(Diagnostic.Create(GULOC03UseResource.Descriptor, context.Node.GetLocation()));

                if (literal.Token.ValueText.Contains("{") &&
                    literal.Token.ValueText.Contains("}"))
                {
                    context.ReportDiagnostic(Diagnostic.Create(GULOC06UseInterpolation.Descriptor, context.Node.GetLocation()));
                }
            }
        }

        private static bool IsExcludedFile(string fileName)
        {
            return !fileName.EndsWith(".cs") ||
                   fileName.EndsWith(".g.cs") ||
                   fileName.EndsWith("Designer.cs") ||
                   fileName.EndsWith("AssemblyInfo.cs");
        }
    }
}
