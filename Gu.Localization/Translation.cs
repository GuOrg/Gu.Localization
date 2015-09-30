namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    [DebuggerDisplay("Key: {_assemblyAndKey} Translated: {Translated}")]
    public class Translation : ITranslation
    {
        private static readonly ConditionalWeakTable<AssemblyAndKey, Translation> Cache = new ConditionalWeakTable<AssemblyAndKey, Translation>();
        private static readonly HashSet<AssemblyAndKey> Keys = new HashSet<AssemblyAndKey>();
        private static readonly object Gate = new object();
        private static readonly PropertyChangedEventArgs TranslatedPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Translated));
        private readonly AssemblyAndKey _assemblyAndKey;

        static Translation()
        {
            Translator.CurrentLanguageChanged += (sender, info) => UpdateTranslations();
        }

        private Translation(AssemblyAndKey assemblyAndKey)
        {
            _assemblyAndKey = assemblyAndKey;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The key Translated to the CurrentCulture
        /// </summary>
        public string Translated => Translator.Translate(_assemblyAndKey.Assembly, _assemblyAndKey.Key);


        public static Translation GetOrCreate(Type typeInAssembly, string key)
        {
            return GetOrCreate(AssemblyAndKey.GetOrCreate(typeInAssembly.Assembly, key));
        }

        public static Translation GetOrCreate(Expression<Func<string>> key)
        {
            return GetOrCreate(AssemblyAndKey.GetOrCreate(key));
        }

        internal static Translation GetOrCreate(AssemblyAndKey key)
        {
            Translation result;
            if (Cache.TryGetValue(key, out result))
            {
                return result;
            }
            lock (Gate)
            {
                if (Cache.TryGetValue(key, out result))
                {
                    return result;
                }
                result = new Translation(key);
                Cache.Add(key, result);
                Keys.Add(key);
                Purge();
            }
            return result;
        }

        public static void Purge()
        {
            lock (Gate)
            {
                Translation temp;
                Keys.RemoveWhere(x => !Cache.TryGetValue(x, out temp));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private static void UpdateTranslations()
        {
            lock (Gate)
            {
                foreach (var key in Keys)
                {
                    Translation translation;
                    if (Cache.TryGetValue(key, out translation))
                    {
                        translation.OnPropertyChanged(TranslatedPropertyChangedEventArgs);
                    }

                    else
                    {
                        Keys.Remove(key);
                    }
                }
            }
        private void OnLanguageChanged(object sender, CultureInfo e)
        {
            OnPropertyChanged(nameof(Translated));
        }
    }
}
