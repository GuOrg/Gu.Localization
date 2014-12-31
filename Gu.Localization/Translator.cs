namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Resources;

    public class Translator : INotifyPropertyChanged
    {
        /// <summary>
        /// This is exposed for binding current language see demo.
        /// </summary>
        public static readonly CultureProxy CultureProxy = new CultureProxy();
        private static readonly List<CultureInfo> _allCultures = new List<CultureInfo>();
        private readonly List<CultureInfo> _cultures = new List<CultureInfo>();
        private static CultureInfo _currentCulture;
        private ResourceManagerWrapper _manager;

        internal Translator(ResourceManagerWrapper manager)
        {
            _manager = manager;
            foreach (var resourceSetAndCulture in manager.ResourceSets)
            {
                var cultureInfo = resourceSetAndCulture.Culture;
                if (AllCultures.All(c => cultureInfo != null && c.TwoLetterISOLanguageName != cultureInfo.TwoLetterISOLanguageName))
                {
                    _allCultures.Add(cultureInfo);
                    OnLanguagesChanged();
                    OnLanguageChanged(cultureInfo);
                }
            }
        }

        public static event EventHandler<CultureInfo> LanguageChanged;
       
        public static event EventHandler<EventArgs> LanguagesChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The culture to translate to
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return _currentCulture ?? AllCultures.FirstOrDefault();
            }
            set
            {
                if (Equals(_currentCulture, value))
                {
                    return;
                }
                _currentCulture = value;
                OnLanguageChanged(value);
            }
        }

        public static IEnumerable<CultureInfo> AllCultures
        {
            get
            {
                return _allCultures;
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

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static void OnLanguageChanged(CultureInfo e)
        {
            var handler = LanguageChanged;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        private static void OnLanguagesChanged()
        {
            var handler = LanguagesChanged;
            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }
    }
}
