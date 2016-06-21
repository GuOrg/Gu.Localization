namespace Gu.Localization.Tests
{
    using System.Collections;
    using System.Reflection;

    public class TestHelpers
    {
        public static void ClearTranslationCache()
        {
            var fieldInfo = typeof(Translation).GetField("Cache", BindingFlags.Static|BindingFlags.NonPublic);
            var cache = (IDictionary)fieldInfo.GetValue(null);
            cache.Clear();
        }
    }
}
