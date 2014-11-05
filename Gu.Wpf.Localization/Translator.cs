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
                FrameworkPropertyMetadataOptions.AffectsArrange|FrameworkPropertyMetadataOptions.AffectsMeasure, 
                OnCultureChanged));

        private readonly ResourceManagerWrapper _manager;
        private readonly string _key;
        private static CultureInfo _currentCulture;
        public static readonly CultureProxy CultureProxy = new CultureProxy();
        internal Translator(ResourceManagerWrapper manager, string key)
        {
            _manager = manager;
            _key = key;
            foreach (var resourceSetAndCulture in manager.ResourceSets)
            {
                var cultureInfo = resourceSetAndCulture.Culture;
                if (Cultures.All(c =>cultureInfo!=null && c.TwoLetterISOLanguageName != cultureInfo.TwoLetterISOLanguageName))
                {
                    Cultures.Add(cultureInfo);
                    OnLanguageCahnged(cultureInfo);
                }
            }
            LanguageCahnged += (sender, info) => OnPropertyChanged("Value");
        }

        public Translator Create<T>(T resources, Func<T, ResourceManager> manager, Expression<Func<T, string>> key)
        {
            var memberExpression = (MemberExpression)key.Body;
            return new Translator(new ResourceManagerWrapper(manager(resources)), memberExpression.Member.Name);
        }

        public static event EventHandler<CultureInfo> LanguageCahnged;

        public event PropertyChangedEventHandler PropertyChanged;

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
            var handler = LanguageCahnged;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static CultureInfo GetCulture(UIElement element)
        {
            return (CultureInfo)element.GetValue(CultureProperty);
        }

        public static void SetCulture(UIElement element, CultureInfo value)
        {
            element.SetValue(CultureProperty, value);
        }

        private static void OnCultureChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            CurrentCulture = (CultureInfo)e.NewValue;
        }
    }
}
