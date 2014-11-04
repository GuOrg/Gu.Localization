namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xaml;
    using Gu.Wpf.Localization.Annotations;

    public class TranslationManager : INotifyPropertyChanged
    {
        private static readonly ConcurrentDictionary<Assembly, TranslationManager> Cache = new ConcurrentDictionary<Assembly, TranslationManager>();
        private static readonly TranslationManager _default = new TranslationManager(new ResxTranslationProvider(Assembly.GetEntryAssembly()));
        private ITranslationProvider _translationProvider;
        public static readonly DependencyProperty DesignModeCultureProperty = DependencyProperty.RegisterAttached(
            "DesignModeCulture",
            typeof(CultureInfo),
            typeof(TranslationManager),
            new PropertyMetadata(CultureInfo.CurrentUICulture, OnDesignModeCultureChanged));

        private TranslationManager()
        {
            LanguageChanged += (sender, info) => OnPropertyChanged("CurrentLanguage");
        }

        private TranslationManager(ITranslationProvider provider)
            : this()
        {
            _translationProvider = provider;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static event EventHandler<CultureInfo> LanguageChanged;

        public static TranslationManager Instance
        {
            get
            {
                var assembly = Assembly.GetCallingAssembly();
                return Cache.GetOrAdd(assembly, a => Create(a));
            }
        }

        public static TranslationManager Default
        {
            get { return _default; }
        }

        public IEnumerable<Assembly> Assemblies { get; private set; }

        public ITranslationProvider TranslationProvider
        {
            get
            {
                return this._translationProvider;
            }
            set
            {
                if (Equals(value, this._translationProvider))
                {
                    return;
                }
                this._translationProvider = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("Languages");
            }
        }

        public CultureInfo CurrentLanguage
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (!Equals(value, Thread.CurrentThread.CurrentUICulture))
                {
                    Thread.CurrentThread.CurrentUICulture = value;
                    OnLanguageChanged(value);
                    this.OnPropertyChanged();
                }
            }
        }

        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                if (this.TranslationProvider != null)
                {
                    return this.TranslationProvider.Languages;
                }

                return Enumerable.Empty<CultureInfo>();
            }
        }

        /// <summary>
        /// Use this to get the translationmanager for another assembly
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static TranslationManager Create(params Assembly[] assemblies)
        {
            const string SystemXaml = "System.Xaml";
            var assemblyList = new List<Assembly>();
            if (assemblies == null || !assemblies.Any())
            {
                assemblyList.Add(Assembly.GetEntryAssembly());
            }
            else
            {
                if (assemblies.Any(a => a.GetName().Name == SystemXaml))
                {
                    assemblyList.Add(Assembly.GetEntryAssembly());
                }
                assemblyList.AddRange(assemblies.Where(x => x.GetName().Name != SystemXaml));
            }
            var manager = new TranslationManager(new ResxTranslationProvider(assemblyList));
            return manager;
        }

        public static TranslationManager Create(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.ProvideValueTarget();
            if (provideValueTarget != null)
            {
                var element = provideValueTarget.TargetObject as FrameworkElement;
                if (element != null && !element.IsLoaded)
                {
                    var translationManager = new TranslationManager();
                    element.Loaded += translationManager.ElementOnLoaded;
                    return translationManager;
                }
            }
            return Default;
        }

        public static CultureInfo GetDesignModeCulture(UIElement element)
        {
            return (CultureInfo)element.GetValue(DesignModeCultureProperty);
        }

        public static void SetDesignModeCulture(UIElement element, CultureInfo value)
        {
            element.SetValue(DesignModeCultureProperty, value);
        }

        public string Translate(string key)
        {
            if (this.TranslationProvider != null)
            {
                try
                {
                    var translated = this.TranslationProvider.Translate(key);
                    if (translated != null)
                    {
                        return translated;
                    }
                }
                catch (Exception)
                {
                    return string.Format(Properties.Resources.UnknownErrorFormat, key);
                }
            }

            return string.Format(Properties.Resources.NullManagerFormat, key);
        }

        public bool HasKey(string key, CultureInfo culture)
        {
            return this.TranslationProvider.HasKey(key, culture);
        }

        public TranslationInfo GetInfo(string key)
        {
            var provider = TranslationProvider as ResxTranslationProvider;
            if (_translationProvider == null)
            {
                return TranslationInfo.NoProvider;
            }
            if (!Languages.Any())
            {

                if (provider == null)
                {
                    return TranslationInfo.NoResources;
                }
                return TranslationInfo.NoLanguages;
            }
            if (_translationProvider.HasKey(key, CultureInfo.CurrentUICulture))
            {
                return TranslationInfo.CanTranslate;
            }
            if (provider == null)
            {
                return TranslationInfo.NoTranslation;
            }
            var translations = provider.ResourceManagers.Select(x => x.ResourceManager.GetString(key));
            if (translations.All(x => x == null)) // Possibly undocumented behavior
            {
                return TranslationInfo.MissingKey;
            }
            return TranslationInfo.NoTranslation;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static void OnLanguageChanged(CultureInfo newLanguage)
        {
            if (LanguageChanged != null)
            {
                LanguageChanged(null, newLanguage);
            }
        }

        private static void OnDesignModeCultureChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (DesignMode.IsInDesignMode)
            {
                Default.CurrentLanguage = (CultureInfo)dependencyPropertyChangedEventArgs.NewValue;
            }
        }

        private void ElementOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var frameworkElement = (FrameworkElement)sender;
            frameworkElement.Loaded -= this.ElementOnLoaded;
            var assemblies = frameworkElement.AncestorsAndSelf()
                                             .Select(x => x.GetType().Assembly)
                                             .Distinct()
                                             .ToArray();
            _translationProvider = new ResxTranslationProvider(assemblies);
            this.OnPropertyChanged("CurrentLanguage"); // Hack to trigger refresh
        }

    }
}