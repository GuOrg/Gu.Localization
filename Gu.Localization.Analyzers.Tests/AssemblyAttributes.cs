using Gu.Roslyn.Asserts;

[assembly: TransitiveMetadataReferences(
    typeof(Gu.Localization.Analyzers.Tests.HappyPathWithAll),
    typeof(Gu.Localization.Translation))]
