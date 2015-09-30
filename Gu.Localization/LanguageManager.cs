namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    public sealed class LanguageManager
    {
        private static readonly ConcurrentDictionary<Assembly, ILanguageManager> Cache = new ConcurrentDictionary<Assembly, ILanguageManager>();

        public static ILanguageManager GetOrCreate(Type typeInAssembly)
        {
            return GetOrCreate(typeInAssembly.Assembly);
        }

        public static ILanguageManager GetOrCreate(Assembly assembly)
        {
            ILanguageManager result;
            if (Cache.TryGetValue(assembly, out result))
            {
                return result;
            }
            result = Cache.GetOrAdd(assembly, a => new FileLanguageManager(a));

            return result;
        }
    }
}
