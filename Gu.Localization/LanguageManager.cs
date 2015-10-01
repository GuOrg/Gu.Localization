using System.Collections.Generic;
using Gu.Localization.Internals;

namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    public sealed class LanguageManager
    {
        private static readonly ConcurrentDictionary<Assembly, ILanguageManager> Cache = new ConcurrentDictionary<Assembly, ILanguageManager>(AssemblyComparer.Default);
        private static readonly ObservableSet<KeyValuePair<Assembly, ILanguageManager>> _assembliesAndLanguages = new ObservableSet<KeyValuePair<Assembly, ILanguageManager>>();

        public static IObservableSet<KeyValuePair<Assembly, ILanguageManager>> AssembliesAndLanguages => _assembliesAndLanguages;

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
            _assembliesAndLanguages.UnionWith(Cache);
            return result;
        }
    }
}
