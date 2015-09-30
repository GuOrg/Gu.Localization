namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Gu.Localization.Properties;

    public class Translator : ITranslator
    {
        private static CultureInfo _currentCulture;

        private static Assembly _executingAssembly;

        private static readonly ObservableCollection<CultureInfo> AllCulturesInner = new ObservableCollection<CultureInfo>();

        public static event EventHandler<CultureInfo> CurrentLanguageChanged;

        static Translator()
        {
            AllCultures = new ReadOnlyObservableCollection<CultureInfo>(AllCulturesInner);
            ExecutingAssembly = Assembly.GetEntryAssembly();
        }

        /// <summary>
        /// The culture to translate to
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                if (_currentCulture != null)
                {
                    return _currentCulture;
                }
                CurrentCulture = AllCultures.FirstOrDefault();
                return _currentCulture ?? CultureInfo.CurrentUICulture;
            }
            set
            {
                if (Equals(_currentCulture, value))
                {
                    return;
                }
                _currentCulture = value;
                OnCurrentCultureChanged(value);
            }
        }

        public static ReadOnlyObservableCollection<CultureInfo> AllCultures { get; }

        public static Assembly ExecutingAssembly
        {
            get
            {
                return _executingAssembly;
            }
            set
            {
                _executingAssembly = value;
                AllCulturesInner.Clear();
                if (_executingAssembly != null)
                {
                    var languages = LanguageManager.GetOrCreate(value)
                             .Languages;
                    foreach (var cultureInfo in languages)
                    {
                        AllCulturesInner.Add(cultureInfo);
                    }
                    if (_currentCulture == null)
                    {
                        CurrentCulture = AllCultures.FirstOrDefault();
                    }
                }
            }
        }

        CultureInfo ITranslator.CurrentCulture
        {
            get
            {
                return CurrentCulture;
            }
            set
            {
                CurrentCulture = value;
            }
        }

        IReadOnlyList<CultureInfo> ITranslator.AllCultures => AllCultures;

        public static string Translate(Expression<Func<string>> key)
        {
            if (ExpressionHelper.IsResourceKey(key))
            {
                var type = ExpressionHelper.GetRootType(key);
                var resourceKey = ExpressionHelper.GetResourceKey(key);
                return Translate(type, resourceKey);
            }
            return key.Compile().Invoke();
        }

        public static string Translate(Type typeInAsembly, string key)
        {
            if (typeInAsembly == null)
            {
                throw new ArgumentNullException(nameof(typeInAsembly));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }

            var translate = Translate(typeInAsembly.Assembly, key);
            if (translate != null)
            {
                return translate;
            }
            return string.Format(Properties.Resources.MissingTranslationFormat, key);
        }

        public static string Translate(Assembly assembly, string key)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }

            var manager = LanguageManager.GetOrCreate(assembly);

            if (manager == null)
            {
                return string.Format(Resources.NullManagerFormat, key);
            }
            if (string.IsNullOrEmpty(key))
            {
                return "null";
            }
            var translated = manager.Translate(CurrentCulture, key);
            if (translated == null)
            {
                return string.Format(Properties.Resources.MissingKeyFormat, key);
            }
            return translated;
        }

        string ITranslator.Translate(Expression<Func<string>> key) => Translate(key);

        string ITranslator.Translate(Type typeInAsembly, string key) => Translate(typeInAsembly, key);

        private static void OnCurrentCultureChanged(CultureInfo e)
        {
            CurrentLanguageChanged?.Invoke(null, e);
        }
    }
}
