namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using System.Xml.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ResourceAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            GULOC07KeyDoesNotMatch.Descriptor);

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
                property.ContainingType is INamedTypeSymbol containingType &&
                containingType.Name == "Resources" &&
                containingType.TryFindProperty("ResourceManager", out _) &&
                Resources.TryGetDefaultResx(containingType, out var resx))
            {
                var xDocument = XDocument.Load(resx.FullName);
                if (xDocument.Root is XElement root)
                {
                    foreach (var candidate in root.Elements("data"))
                    {
                        if (candidate.Attribute("name") is XAttribute attribute &&
                            attribute.Value == property.Name &&
                            candidate.Element("value") is XElement value &&
                            Resources.TryGetKey(value.Value, out var key) &&
                            key != property.Name)
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    GULOC07KeyDoesNotMatch.Descriptor,
                                    propertyDeclaration.Identifier.GetLocation(),
                                    ImmutableDictionary<string, string>.Empty.Add("Key", key),
                                    key));
                        }
                    }
                }
            }
        }
    }
}
