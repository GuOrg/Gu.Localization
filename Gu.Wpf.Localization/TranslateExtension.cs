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
        private TranslationManager _translationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public TranslateExtension(string key)
        {
            if (DesignMode.IsInDesignMode && key == null)
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
                    if (DesignMode.IsInDesignMode)
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