namespace Gu.Localization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Gu.Localization.Internals;
    using Gu.Localization.Properties;
    using JetBrains.Annotations;

    public class Translator : ITranslator
    {
        public static readonly Translator Instance = new Translator();
        internal static readonly ObservableSet<CultureInfo> AllCulturesInner = new ObservableSet<CultureInfo>(CultureInfoComparer.Default);
        internal static readonly ObservableSet<AssemblyAndLanguages> AllAssembliesAndLanguagesInner = new ObservableSet<AssemblyAndLanguages>();
        private static CultureInfo _currentCulture = CultureInfo.InvariantCulture;

        static Translator()
        {
            Factory = FileLanguageManager.Factory;
        }

        private Translator()
        {
            CurrentLanguageChanged += (_, __) => OnPropertyChanged(nameof(CurrentCulture));
        }

        public static event EventHandler<CultureInfo> CurrentLanguageChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The culture to translate to
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return _currentCulture;
            }
            set
            {
                if (CultureInfoComparer.Default.Equals(_currentCulture, value))
                {
                    return;
                }
                _currentCulture = value ?? CultureInfo.InvariantCulture;
                OnCurrentCultureChanged(_currentCulture);
            }
        }

        public static IObservableSet<CultureInfo> AllCultures => AllCulturesInner;

        public static IObservableSet<AssemblyAndLanguages> AllAssembliesAndLanguages => AllAssembliesAndLanguagesInner;

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

        public static ILanguageManagerFactory Factory { get; }

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
            if (typeInAssembly == null)
            {
                throw new ArgumentNullException(nameof(typeInAssembly));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }

            return Translate(typeInAssembly.Assembly, key);
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

            var manager = Factory.GetOrCreate(assembly);

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

        string ITranslator.Translate(Type typeInAssembly, string key) => Translate(typeInAssembly, key);

        private static void OnCurrentCultureChanged(CultureInfo e)
        {
            CurrentLanguageChanged?.Invoke(null, e);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
