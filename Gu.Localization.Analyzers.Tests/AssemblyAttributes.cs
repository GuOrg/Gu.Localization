using System;

using Gu.Roslyn.Asserts;

[assembly: CLSCompliant(false)]

[assembly: TransitiveMetadataReferences(
    typeof(Gu.Localization.Analyzers.Tests.AllValid),
    typeof(Gu.Localization.Translation))]
