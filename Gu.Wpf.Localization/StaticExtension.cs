namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Localization;
    using Gu.Localization.Properties;
    using Gu.Wpf.Localization.Designtime;
    using Gu.Wpf.Localization.Internals;

    /// <summary>
    /// Implements a markup extension that translates resources.
    /// The reason for the name StaticExtension is that it tricks Resharper into providing Intellisense.
    /// l:Static p:Resources.YourKey
    /// </summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    [ContentProperty("Member"), DefaultProperty("Member")]
    //[TypeConverter(typeof(StaticExtensionConverter))]
    public class StaticExtension : System.Windows.Markup.StaticExtension
    {
        private ITranslation _translation;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Gu.Wpf.Localization.StaticExtension"/> class using the provided <paramref name="member"/> string.
        /// </summary>
        /// <param name="member">
        /// A string that identifies the member to make a reference to. This string uses the format prefix:typeName.fieldOrPropertyName. prefix is the mapping prefix for a XAML namespace, and is only required to reference static values that are not mapped to the default XAML namespace.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="member"/> is null.
        /// </exception>
        public StaticExtension(string member)
        {
            Member = member;
        }

        [ConstructorArgument("member")]
        public new string Member { get; set; }

        /// <summary>
        /// Returns an object value to set on the property where you apply this extension. For <see cref="T:System.Windows.Markup.StaticExtension"/>, the return value is the static value that is evaluated for the requested static member.
        /// </summary>
        /// <returns>
        /// The static value to set on the property where the extension is applied.
        /// </returns>
        /// <param name="serviceProvider">
        /// An object that can provide services for the markup extension. The service provider is expected to provide a service that implements a type resolver (<see cref="T:System.Windows.Markup.IXamlTypeResolver"/>).
        /// </param>
        /// <exception cref="T:System.InvalidOperationException">
        /// The <paramref name="Member"/> value for the extension is null at the time of evaluation.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// Some part of the <paramref name="member"/> string did not parse properly-or-<paramref name="serviceProvider"/> did not provide a service for <see cref="T:System.Windows.Markup.IXamlTypeResolver"/>-or-<paramref name="member"/> value did not resolve to a static member.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="serviceProvider"/> is null.
        /// </exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Member))
            {
                throw new InvalidOperationException("MarkupExtensionStaticMember");
            }

            try
            {
                var qnk = QualifiedNameAndKey.Parse(Member);
                if (qnk.QualifiedName == null || qnk.Key == null)
                {
                    _translation = new TranslationInfo(string.Format(Gu.Localization.Properties.Resources.UnknownErrorFormat, Member));
                }
                else
                {
                    var key = GetAssemblyAndKey(serviceProvider, qnk);
                    if (key == null)
                    {
                        _translation = new TranslationInfo(string.Format(Resources.MissingKeyFormat, Member));
                    }
                    else
                    {
                        _translation = Translation.GetOrCreate(key);
                    }
                }
            }
            catch (Exception exception)
            {
                _translation = Design.IsDesignMode
                                   ? new TranslationInfo(exception.Message)
                                   : new TranslationInfo(string.Format(Resources.UnknownErrorFormat, Member));
            }

            var binding = new Binding(nameof(_translation.Translated))
            {
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Source = _translation
            };
            var provideValue = binding.ProvideValue(serviceProvider);
            return provideValue;
        }

        internal static AssemblyAndKey GetAssemblyAndKey(IServiceProvider serviceProvider, QualifiedNameAndKey qnk)
        {
            var type = serviceProvider.Resolve(qnk.QualifiedName);
            return AssemblyAndKey.GetOrCreate(type.Assembly, qnk.Key);
        }
    }
}
