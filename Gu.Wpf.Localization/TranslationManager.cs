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
    using System.Xaml;
    using Gu.Wpf.Localization.Annotations;

    public class TranslationManager : INotifyPropertyChanged
    {
        private ITranslationProvider _translationProvider;

        private TranslationManager()
        {
            LanguageChanged += (sender, info) => OnPropertyChanged("CurrentLanguage");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static event EventHandler<CultureInfo> LanguageChanged;

        public static TranslationManager Instance
        {
            get
            {
                var assembly = Assembly.GetCallingAssembly();
                return Create(assembly);
            }
        }

        public Assembly[] Assemblies { get; private set; }

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
            const string systemXaml = "System.Xaml";
            var assemblyList = new List<Assembly>();
            if (assemblies == null || !assemblies.Any())
            {
                assemblyList.Add(Assembly.GetEntryAssembly());
            }
            else
            {
                if (assemblies.Any(a => a.GetName().Name == systemXaml))
                {
                    assemblyList.Add(Assembly.GetEntryAssembly());
                }
                assemblyList.AddRange(assemblies.Where(x => x.GetName().Name != systemXaml));
            }
            var manager = CreateManager(assemblyList);
            return manager;
        }

        public static TranslationManager Create(IServiceProvider serviceProvider)
        {
            var assemblies = new List<Assembly>();
            var rootObject = serviceProvider.RootObjectProvider();
            var provideValueTarget = serviceProvider.ProvideValueTarget();
            var typeDescriptorContext = serviceProvider.TypeDescriptorContext();
            var uriContext = serviceProvider.UriContext();
            var provider = serviceProvider.ServiceProvider();
            if (rootObject != null)
            {
                if (rootObject.RootObject is ResourceDictionary)
                {
                    //var provider = (IServiceProvider)rootObject;
                    //var service = (IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget));
                    //var controltemplate = service.TargetObject as ControlTemplate;
                    var assemblies1 = AppDomain.CurrentDomain.GetAssemblies().Reverse().ToArray();
                    foreach (var assembly in assemblies1)
                    {
                        string[] manifestResourceNames = assembly.GetManifestResourceNames();
                        foreach (var manifestResourceName in manifestResourceNames)
                        {
                            ManifestResourceInfo manifestResourceInfo = assembly.GetManifestResourceInfo(manifestResourceName);
                        }
                    }

                    //assemblies.Add(controltemplate.TargetType.Assembly);
                    assemblies.Add(Assembly.GetEntryAssembly());
                }
                if (rootObject.RootObject.GetType().IsSubclassOf(typeof(Window)))
                {
                    assemblies.Add(rootObject.RootObject.GetType().Assembly);
                }
                if (rootObject.RootObject.GetType().IsSubclassOf(typeof(Application)))
                {
                    assemblies.Add(rootObject.RootObject.GetType().Assembly);
                }
            }
            else
            {
                throw new NotImplementedException("message");
            }
            return Create(assemblies.ToArray());
        }

        public string Translate(string key)
        {
            if (this.TranslationProvider != null)
            {
                try
                {
                    string translatedValue = this.TranslationProvider.Translate(key);
                    if (translatedValue != null)
                    {
                        return translatedValue;
                    }
                }
                catch (Exception)
                {
                    return string.Format("!{0}!", key);
                }
            }

            return string.Format("!{0}!", key);
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
                    //var manifestResourceNames = Assembly.GetManifestResourceNames();
                    //if (provider.ResourceManager.GetResourceSet())
                    //{

                    //}
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
            var s = provider.ResourceManagers.FirstOrDefault(x => !string.IsNullOrEmpty(x.GetString(key, CurrentLanguage)));
            if (s == null) // Possibly undocumented behavior
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

        private static TranslationManager CreateManager(List<Assembly> assemblies)
        {
            var resourceManagers = assemblies.Select(x => new ResourceManager(x.GetName().Name + ".Properties.Resources", x));
            return new TranslationManager
                              {
                                  Assemblies = assemblies.ToArray(),
                                  TranslationProvider = new ResxTranslationProvider(resourceManagers)
                              };
        }

        private static void OnLanguageChanged(CultureInfo newLanguage)
        {
            if (LanguageChanged != null)
            {
                LanguageChanged(null, newLanguage);
            }
        }
    }
}