namespace Gu.Localization.Analyzers.Tests.Helpers
{
    using Microsoft.CodeAnalysis;

    internal static class TextDocumentExt
    {
        internal static string GetText(this TextDocument file) => file.GetTextAsync().GetAwaiter().GetResult().ToString();
    }
}
