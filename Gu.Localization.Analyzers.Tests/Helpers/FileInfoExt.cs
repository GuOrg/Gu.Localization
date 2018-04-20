namespace Gu.Localization.Analyzers.Tests.Helpers
{
    using System.IO;
    using Gu.Roslyn.Asserts;

    internal static class FileInfoExt
    {
        internal static void ReplaceText(this FileInfo file, string oldText, string newText)
        {
            File.WriteAllText(file.FullName, File.ReadAllText(file.FullName).AssertReplace(oldText, newText));
        }
    }
}
