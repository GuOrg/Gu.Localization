namespace Gu.Wpf.Localization
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Gu.Localization;

    /// <summary>MarkupExtension for binging to <see cref="Translator.CurrentCulture"/>.</summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class CurrentCultureExtension : MarkupExtension
    {
        private static readonly PropertyPath ValuePath = new PropertyPath(nameof(CurrentCultureProxy.Value));

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding()
            {
                Source = CurrentCultureProxy.Instance,
                Path = ValuePath,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay
            };

            return binding.ProvideValue(serviceProvider);
        }
    }
}