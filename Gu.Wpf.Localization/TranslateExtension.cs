namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
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
            var provideValueTarget = serviceProvider.ProvideValueTarget();
            if (!(provideValueTarget.TargetObject is DependencyObject))
            {
                return this;
            }
            if (_translationManager == null)
            {
                try
                {
                    _translationManager = TranslationManager.Create(serviceProvider);
                }
                catch (Exception e)
                {
                    if (IsDesigntime)
                    {
                        throw;
                    }
                }
            }

            var translationData = new TranslationData(this.Key, _translationManager);
            var binding = new Binding("Value")
            {
                Source = translationData
            };
            var provideValue = binding.ProvideValue(serviceProvider);
            return provideValue;
        }
    }
}