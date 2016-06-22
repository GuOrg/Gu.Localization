namespace Gu.Wpf.Localization
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>MarkupExtension for binging to <see cref="Gu.Localization.Translator.EffectiveCulture"/>.</summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class EffectiveCultureExtension : MarkupExtension
    {
        private static readonly PropertyPath ValuePath = new PropertyPath(nameof(EffectiveCultureProxy.Value));

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding()
            {
                Source = EffectiveCultureProxy.Instance,
                Path = ValuePath,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay
            };

            return binding.ProvideValue(serviceProvider);
        }
    }
}