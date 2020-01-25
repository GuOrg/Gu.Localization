namespace Gu.Localization.Tests
{
    using System.Collections;
    using System.Reflection;

    public static class TestHelpers
    {
        public static void ClearTranslationCache()
        {
            var fieldInfo = typeof(Translation).GetField("Cache", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
            var cache = (IDictionary)fieldInfo.GetValue(null);
            cache.Clear();
        }
    }
}
