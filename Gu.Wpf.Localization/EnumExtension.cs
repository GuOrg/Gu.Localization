namespace Gu.Wpf.Localization
{
    using System;
    using System.Resources;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Localization;
    using Gu.Localization.Properties;

    /// <summary>
    /// A markup extension that translates enum members resources.
    /// Usage: Text="{l:Enum ResourceManager={x:Static p:Resources.ResourceManager}, Member={x:Static local:DummyEnum.TranslatedToAll}}"
    /// </summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class EnumExtension : MarkupExtension
    {
        /// <summary>Gets or sets the <see cref="ResourceManager"/> with translations for <see cref="Member"/>.</summary>
        public ResourceManager ResourceManager { get; set; }

        /// <summary>Gets or sets the enum member</summary>
        public IFormattable Member { get; set; }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                if (this.ResourceManager == null || this.Member == null)
                {
                    return $"{nameof(EnumExtension)} must have {nameof(this.ResourceManager)} and {nameof(this.Member)}";
                }

                if (!this.Member.GetType().IsEnum)
                {
                    return $"{nameof(EnumExtension)} {nameof(this.Member)} must be an enum";
                }

                return CreateBindingExpression(this.ResourceManager, this.Member.ToString(), serviceProvider);
            }
            catch (Exception)
            {
                return string.Format(Resources.UnknownErrorFormat, this.Member);
            }
        }

        private static object CreateBindingExpression(ResourceManager resourceManager, string key, IServiceProvider serviceProvider)
        {
            var translation = Translation.GetOrCreate(resourceManager, key);
            var binding = new Binding(nameof(translation.Translated))
            {
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Source = translation
            };

            var provideValue = binding.ProvideValue(serviceProvider);
            return provideValue;
        }
    }
}