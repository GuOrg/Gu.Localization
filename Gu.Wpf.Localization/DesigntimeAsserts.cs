namespace Gu.Wpf.Localization
{
    using System;
    using System.Linq;

    public static class DesigntimeAsserts
    {
        private static readonly ResxTranslationProvider Provider = new ResxTranslationProvider(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic));
        public static void AssertTranslation(string key)
        {
            //if (!Provider.HasKey(key, null))
            //{
            //    throw new ArgumentException(string.Format("No translation for {0}", key));
            //}
        }
    }
}
