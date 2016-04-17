namespace Gu.Localization
{
    internal static class StringExt
    {
        internal static string Slice(this string text, int start, int end)
        {
            return text.Substring(start, end - start + 1);
        }
    }
}
