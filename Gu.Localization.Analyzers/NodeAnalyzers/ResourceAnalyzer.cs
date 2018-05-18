namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ResourceAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            GULOC07KeyDoesNotMatch.Descriptor,
            GULOC08DuplicateNeutral.Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Handle, SyntaxKind.PropertyDeclaration);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            if (context.Node is PropertyDeclarationSyntax propertyDeclaration &&
                context.ContainingSymbol is IPropertySymbol property &&
                property.Type == KnownSymbol.String &&
                ResxFile.TryGetDefault(property.ContainingType, out var resx) &&
                resx.TryGetString(property.Name, out var text))
            {
                if (Resources.TryGetKey(text, out var key) &&
                    key != property.Name)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GULOC07KeyDoesNotMatch.Descriptor,
                            propertyDeclaration.Identifier.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add("Key", key),
                            key));
                }

                foreach (var data in resx.Document.Root.Elements("data"))
                {
                    if (ResxFile.TryGetString(data, out var candidateText) &&
                        candidateText == text &&
                        ResxFile.TryGetName(data, out var candidateName) &&
                        candidateName != property.Name)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                GULOC08DuplicateNeutral.Descriptor,
                                propertyDeclaration.Identifier.GetLocation(),
                                text));
                    }
                }
            }
        }
    }
}
