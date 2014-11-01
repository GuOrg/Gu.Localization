namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Xaml;

    /// <summary>
    /// The Translate Markup extension returns a binding to a TranslationData
    /// that provides a translated resource of the specified key
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    ////[Localizability(LocalizationCategory.Text)]
    public class TranslateExtension : MarkupExtension
    {
        private static readonly DependencyObject DependencyObject = new DependencyObject();
        private static readonly ConcurrentDictionary<AppDomain, Assembly> DesignTimeCache = new ConcurrentDictionary<AppDomain, Assembly>();
        private static readonly ConcurrentDictionary<Uri, Assembly> RunTimeCache = new ConcurrentDictionary<Uri, Assembly>();
        private TranslationManager _translationManager;
        public bool? TestIsDesigntime = null; // Hacking it ugly like this to be able to test

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public TranslateExtension(string key)
        {
            if (IsDesigntime && key == null)
            {
                throw new ArgumentNullException("key");
            }
            this.Key = key;
        }

        /// <summary>
        /// 
        /// </summary>
        [ConstructorArgument("key")]
        public string Key { get; set; }

        /// <summary>
        /// Check if is in desigtime mode
        /// </summary>
        public bool IsDesigntime
        {
            get
            {
                if (TestIsDesigntime.HasValue)
                {
                    return TestIsDesigntime.Value;
                }
                return DesignerProperties.GetIsInDesignMode(DependencyObject);
            }
        }

        /// <summary>
        /// See <see cref="MarkupExtension.ProvideValue" />
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_translationManager == null)
            {
                try
                {
                    _translationManager = this.GetTranslationManager(serviceProvider);
                }
                catch (Exception)
                {
                    if (IsDesigntime)
                    {
                        throw;
                    }
                }
            }

            var translationData = IsDesigntime
                                      ? (ITranslationData)new TranslationDataDesigntime(Key, _translationManager)
                                      : new TranslationData(this.Key, _translationManager);
            var binding = new Binding("Value")
            {
                Source = translationData
            };
            var provideValue = binding.ProvideValue(serviceProvider);
            return provideValue;
        }

        private TranslationManager GetTranslationManager(IServiceProvider serviceProvider)
        {
            List<Assembly> assemblies = new List<Assembly>();
            if (this.IsDesigntime)
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentException("serviceProvider == null");
                }
                assemblies.Add(this.GetDesigntimeRootAssembly());
            }
            else
            {
                var uriContext = (IUriContext)serviceProvider.GetService(typeof(IUriContext));
                var rootObject = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
                if (rootObject != null)
                {
                    assemblies.Add(rootObject.RootObject.GetType().Assembly);
                }
                if (uriContext.BaseUri == null) // this means controltemplate
                {
                    throw new NotImplementedException("message");
                }
            }
            return TranslationManager.GetInstance(assemblies);
        }

        private Assembly GetAssemblyFromUri(Uri uri)
        {
            var localPath = uri.LocalPath;
            var startIndex = 1;
            var endIndex = localPath.IndexOf(";");
            string assemblyName = localPath.Substring(startIndex, endIndex - startIndex);
            return Assembly.Load(assemblyName);
        }

        /// <summary>
        /// This ugly workaround is needed since IRootObjectProvider is null in designtime
        /// </summary>
        /// <returns></returns>
        private Assembly GetDesigntimeRootAssembly()
        {
            return DesignTimeCache.GetOrAdd(AppDomain.CurrentDomain, ad => ad.GetAssemblies().Last(a => a.EntryPoint != null));
        }
    }
}