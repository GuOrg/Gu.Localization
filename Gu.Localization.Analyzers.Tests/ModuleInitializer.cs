namespace Gu.Localization.Analyzers.Tests
{
    using System.Runtime.CompilerServices;

    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;

    internal static class ModuleInitializer
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            Settings.Default = Settings.Default
                                       .WithCompilationOptions(x => x.WithSuppressedDiagnostics("CS1701", "CS8019")
                                                                                         .WithNullableContextOptions(NullableContextOptions.Disable))
                                       .WithMetadataReferences(MetadataReferences.Transitive(
                                           typeof(Gu.Localization.Translation),
                                           typeof(System.Windows.Window)));
        }
    }
}
