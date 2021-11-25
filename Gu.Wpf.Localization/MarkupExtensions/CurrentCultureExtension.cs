namespace Gu.Wpf.Localization
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Localization;

    /// <summary>MarkupExtension for binging to <see cref="Translator.CurrentCulture"/>.</summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class CurrentCultureExtension : MarkupExtension
    {
        /// <summary>
        /// For binding to the static property <see cref="Translator.CurrentCulture"/>.
        /// </summary>
        internal static readonly PropertyPath TranslatorCurrentCulturePath = new(
            "(0)",
            typeof(Translator).GetProperty(nameof(Translator.CurrentCulture), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly) ?? throw new InvalidOperationException("Did not find property Translator.CurrentCulture."));

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding
            {
                Path = TranslatorCurrentCulturePath,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay,
            };

            return binding.ProvideValue(serviceProvider);
        }
    }
}
