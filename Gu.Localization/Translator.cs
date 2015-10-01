namespace Gu.Localization
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Gu.Localization.Internals;
    using Gu.Localization.Properties;

    public class Translator : ITranslator
    {
        private static CultureInfo _currentCulture;
        internal static readonly ObservableSet<CultureInfo> AllCulturesInner = new ObservableSet<CultureInfo>();

        public static event EventHandler<CultureInfo> CurrentLanguageChanged;

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
                return CultureInfo.InvariantCulture;
            }
            set
            {
                if (Equals(_currentCulture, value))
                {
                    return;
                }
                _currentCulture = value == null
                    ? CultureInfo.InvariantCulture
                    : AllCultures.FirstOrDefault(x => Equals(x, value));

                OnCurrentCultureChanged(value);
            }
        }

        public static IObservableSet<CultureInfo> AllCultures => AllCulturesInner;

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

        IObservableSet<CultureInfo> ITranslator.AllCultures => AllCultures;

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

        public static string Translate(Type typeInAssembly, string key)
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
            return string.Format(Resources.MissingTranslationFormat, key);
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
                return string.Format(Resources.MissingKeyFormat, key);
            }
            return translated;
        }

        string ITranslator.Translate(Expression<Func<string>> key) => Translate(key);

        string ITranslator.Translate(Type typeInAssembly, string key) => Translate(typeInAsembly, key);

        private static void OnCurrentCultureChanged(CultureInfo e)
        {
            CurrentLanguageChanged?.Invoke(null, e);
        }
    }
}
