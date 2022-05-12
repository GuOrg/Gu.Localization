namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using System.Text.RegularExpressions;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ResourceAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.GULOC07KeyDoesNotMatch,
            Descriptors.GULOC08DuplicateNeutral,
            Descriptors.GULOC09Duplicate,
            Descriptors.GULOC10MissingTranslation);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.PropertyDeclaration);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (!context.IsExcludedFromAnalysis() &&
                context.Node is PropertyDeclarationSyntax propertyDeclaration &&
                context.ContainingSymbol is IPropertySymbol { Type.SpecialType: SpecialType.System_String } property &&
                ResxFile.TryGetDefault(property.ContainingType, out var resx) &&
                resx.TryGetString(property.Name, out var neutral))
            {
                if (Resources.TryGetKey(neutral, out var key) &&
                    key != property.Name)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.GULOC07KeyDoesNotMatch,
                            propertyDeclaration.Identifier.GetLocation(),
                            ImmutableDictionary<string, string?>.Empty.Add("Key", key),
                            key));
                }

                foreach (var data in resx.Document.Root!.Elements("data"))
                {
                    if (ResxFile.TryGetString(data, out var candidateText) &&
                        candidateText == neutral &&
                        ResxFile.TryGetName(data, out var candidateName) &&
                        candidateName != property.Name)
                    {
                        if (IsIdentical(resx, property.Name, candidateName))
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    Descriptors.GULOC09Duplicate,
                                    propertyDeclaration.Identifier.GetLocation(),
                                    neutral));
                        }
                        else
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    Descriptors.GULOC08DuplicateNeutral,
                                    propertyDeclaration.Identifier.GetLocation(),
                                    neutral));
                        }
                    }
                }

                foreach (var cultureResx in resx.CultureSpecific())
                {
                    if (!cultureResx.TryGetString(property.Name, out _))
                    {
                        var culture = Regex.Match(cultureResx.FileName, @"\.(?<culture>[^.]+)\.resx", RegexOptions.IgnoreCase | RegexOptions.RightToLeft | RegexOptions.Singleline)
                                           .Groups["culture"]
                                           .Value;
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Descriptors.GULOC10MissingTranslation,
                                propertyDeclaration.Identifier.GetLocation(),
                                culture,
                                neutral));
                    }
                }
            }
        }

        private static bool IsIdentical(ResxFile resx, string key1, string key2)
        {
            foreach (var cultureResx in resx.CultureSpecific())
            {
                if (cultureResx.TryGetString(key1, out var string1))
                {
                    if (cultureResx.TryGetString(key2, out var string2))
                    {
                        if (string1 != string2)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (cultureResx.TryGetString(key2, out _))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
