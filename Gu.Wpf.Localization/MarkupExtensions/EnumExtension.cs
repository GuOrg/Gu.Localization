namespace Gu.Wpf.Localization
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Localization.Properties;

    /// <summary>
    /// A markup extension that translates enum members resources.
    /// Usage: Text="{l:Enum ResourceManager={x:Static p:Resources.ResourceManager}, Member={x:Static local:DummyEnum.TranslatedToAll}}".
    /// </summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class EnumExtension : MarkupExtension
    {
        /// <summary>Gets or sets the <see cref="ResourceManager"/> with translations for <see cref="Member"/>.</summary>
        public ResourceManager? ResourceManager { get; set; }

        /// <summary>Gets or sets the enum member.</summary>
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

                return StaticExtension.CreateBindingExpression(this.ResourceManager, this.Member.ToString(), serviceProvider);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return string.Format(CultureInfo.InvariantCulture, Resources.UnknownErrorFormat, this.Member);
            }
        }
    }
}
