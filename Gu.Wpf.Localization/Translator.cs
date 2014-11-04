namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Resources;

    public class Translator : INotifyPropertyChanged
    {
        private readonly ResourceManager _manager;
        private readonly string _key;
        private static CultureInfo _currentCulture;
        private static readonly CultureProxy CultureProxy = new CultureProxy();
        internal Translator(ResourceManager manager, string key)
        {
            _manager = manager;
            _key = key;
            LanguageCahnged += (sender, info) => OnPropertyChanged("Value");
        }

        public Translator Create<T>(T resources, Func<T, ResourceManager> manager, Expression<Func<T, string>> key)
        {
            var memberExpression = (MemberExpression)key.Body;
            return new Translator(manager(resources), memberExpression.Member.Name);
        }

        public static event EventHandler<CultureInfo> LanguageCahnged;

        public event PropertyChangedEventHandler PropertyChanged;

        public static CultureProxy Culture
        {
            get { return CultureProxy; }
        }

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

        public static ObservableCollection<CultureInfo> Cultures = new ObservableCollection<CultureInfo>
        {
            CultureInfo.GetCultureInfo("sv"),
            CultureInfo.GetCultureInfo("en")
        };

        public string Value
        {
            get
            {
                return _manager.GetString(_key, CurrentCulture);
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
    }
}
