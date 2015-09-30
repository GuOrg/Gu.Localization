namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Reflection;

    public sealed class LanguageManager
    {
        private static readonly ConcurrentDictionary<Assembly, ILanguageManager> Cache = new ConcurrentDictionary<Assembly, ILanguageManager>();
        private static readonly ObservableCollection<CultureInfo> AllCulturesInner = new ObservableCollection<CultureInfo>(); 
        internal static readonly ReadOnlyObservableCollection<CultureInfo> AllCultures = new ReadOnlyObservableCollection<CultureInfo>(AllCulturesInner);

        public static event EventHandler<EventArgs> LanguagesChanged;

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

            foreach (var culture in result.Languages)
            {
                if (!string.IsNullOrEmpty(culture.Name) && !AllCultures.Contains(culture))
                {
                    lock (AllCulturesInner)
                    {
                        if (!string.IsNullOrEmpty(culture.Name) && !AllCultures.Contains(culture))
                        {
                            AllCulturesInner.Add(culture);
                            OnLanguagesChanged();
                        }
                    }
                }
            }
            return result;
        }

        private static void OnLanguagesChanged()
        {
            LanguagesChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
