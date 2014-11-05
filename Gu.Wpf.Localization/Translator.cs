namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Resources;
    using System.Windows;

    public class Translator : INotifyPropertyChanged
    {
        public static ObservableCollection<CultureInfo> Cultures = new ObservableCollection<CultureInfo>();
        public static readonly DependencyProperty CultureProperty = DependencyProperty.RegisterAttached(
            "Culture",
            typeof(CultureInfo),
            typeof(Translator),
            new FrameworkPropertyMetadata(
                CultureInfo.CurrentUICulture,
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnCultureChanged));
        /// <summary>
        /// This is exposed for binding current language see demo.
        /// </summary>
        public static readonly CultureProxy CultureProxy = new CultureProxy();

        private readonly ResourceManagerWrapper _manager;
        private readonly string _key;
        private static CultureInfo _currentCulture;

        internal Translator(ResourceManagerWrapper manager, string key)
        {
            _manager = manager;
            _key = key;
            foreach (var resourceSetAndCulture in manager.ResourceSets)
            {
                var cultureInfo = resourceSetAndCulture.Culture;
                if (Cultures.All(c => cultureInfo != null && c.TwoLetterISOLanguageName != cultureInfo.TwoLetterISOLanguageName))
                {
                    Cultures.Add(cultureInfo);
                    OnLanguageCahnged(cultureInfo);
                }
            }
            LanguageChanged += (sender, info) => OnPropertyChanged("Value");
        }

        public static event EventHandler<CultureInfo> LanguageChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The culture to translate to
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return _currentCulture ?? Cultures.FirstOrDefault();
            }
            set
            {
                if (Equals(_currentCulture, value))
                {
                    return;
                }
                _currentCulture = value;
                OnLanguageCahnged(value);
            }
        }

        /// <summary>
        /// The Value for the key in CurrentCulture
        /// </summary>
        public string Value
        {
            get
            {
                var value = _manager.ResourceManager.GetString(_key, CurrentCulture);
                if (value == null)
                {
                    return string.Format(Properties.Resources.MissingKeyFormat, _key);
                }
                return value;
            }
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, () => Properties.Resources.AllLanguages);
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="key">() => Properties.Resources.AllLanguages</param>
        /// <returns>The key translated to the CurrentCulture</returns>
        public static string Translate(ResourceManager resourceManager, Expression<Func<string>> key)
        {
            var memberExpression = (MemberExpression)key.Body;
            var keyName = memberExpression.Member.Name;
            return resourceManager.GetString(keyName, CurrentCulture);
        }

        public static string Translate(ResourceManager resourceManager, string key)
        {
            return resourceManager.GetString(key, CurrentCulture);
        }

        public static CultureInfo GetCulture(UIElement element)
        {
            return (CultureInfo)element.GetValue(CultureProperty);
        }

        public static void SetCulture(UIElement element, CultureInfo value)
        {
            element.SetValue(CultureProperty, value);
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static void OnLanguageCahnged(CultureInfo e)
        {
            var handler = LanguageChanged;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        private static void OnCultureChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.IsDesignMode)
            {
                var frameworkElement = o as FrameworkElement;
                if (frameworkElement != null && !frameworkElement.IsLoaded)
                {
                    frameworkElement.Loaded += (sender, args) =>
                    {
                        CurrentCulture = null;
                        CurrentCulture = (CultureInfo)e.NewValue;
                    };
                }
                if (frameworkElement != null && !frameworkElement.IsVisible)
                {
                    frameworkElement.IsVisibleChanged += (sender, args) =>
                    {
                        CurrentCulture = null;
                        CurrentCulture = (CultureInfo)e.NewValue;
                    };
                }
            }
            else
            {
                CurrentCulture = (CultureInfo)e.NewValue;
            }
        }
    }
}
