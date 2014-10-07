namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

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

        private Assembly _assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public TranslateExtension(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            this.Key = key;
        }

        [ConstructorArgument("key")]
        public string Key { get; set; }

        public bool IsDesigntime
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(DependencyObject);
            }
        }

        /// <summary>
        /// Checks that there is a translation for Key in all languages.
        /// Used to provide feedback in designtime
        /// </summary>
        /// <param name="key"></param>
        /// <param name="translationManager"></param>
        public void AssertTranslation(string key, TranslationManager translationManager)
        {
            if (translationManager.TranslationProvider == null)
            {
                throw new Exception("translationManager.TranslationProvider == null");
            }

            if (!translationManager.Languages.Any())
            {
                throw new Exception(string.Format("No languages found in assembly: {0}", this._assembly.GetName().Name));
            }

            var missing = translationManager.Languages.Where(x => !translationManager.HasKey(key, x))
                                            .ToArray();
            if (missing.Any())
            {
                var languages = string.Join(", ", missing.Select(x => x.TwoLetterISOLanguageName));
                throw new Exception(string.Format("Translation for: '{0}' is missing in {{{1}}}", key, languages));
            }
        }

        /// <summary>
        /// See <see cref="MarkupExtension.ProvideValue" />
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this._translationManager == null)
            {
                if (serviceProvider == null)
                {
                    throw new Exception("serviceProvider == null");
                }
                if (this.IsDesigntime)
                {
                    this._assembly = this.GetDesigntimeRootAssembly();
                }
                else
                {
                    var uriContext = (IUriContext)serviceProvider.GetService(typeof(IUriContext));
                    this._assembly = RunTimeCache.GetOrAdd(uriContext.BaseUri, this.GetAssemblyFromUri);
                }
                this._translationManager = TranslationManager.GetInstance(this._assembly);
                if (this.IsDesigntime)
                {
                    this.AssertTranslation(this.Key, this._translationManager);
                }
            }

            var binding = new Binding("Value")
                          {
                              Source = new TranslationData(this.Key, this._translationManager)
                          };
            return binding.ProvideValue(serviceProvider);
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