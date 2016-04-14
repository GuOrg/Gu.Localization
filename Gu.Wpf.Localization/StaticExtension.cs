namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Resources;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Localization;
    using Gu.Localization.Properties;

    /// <summary>
    /// Implements a markup extension that translates resources.
    /// The reason for the name StaticExtension is that it tricks Resharper into providing Intellisense.
    /// l:Static p:Resources.YourKey
    /// </summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    [ContentProperty("Member")]
    [DefaultProperty("Member")]
    [TypeConverter(typeof(StaticExtensionConverter))]
    public class StaticExtension : System.Windows.Markup.StaticExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticExtension"/> class using the provided <paramref name="member"/> string.
        /// </summary>
        /// <param name="member">
        /// A string that identifies the member to make a reference to. This string uses the format prefix:typeName.fieldOrPropertyName. prefix is the mapping prefix for a XAML namespace, and is only required to reference static values that are not mapped to the default XAML namespace.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="member"/> is null.
        /// </exception>
        public StaticExtension(string member)
        {
            this.Member = member;
        }

        public StaticExtension(string member, ResourceManager resourceManager)
        {
            this.Member = member;
            this.ResourceManager = resourceManager;
        }

        [ConstructorArgument("member")]
        public string Member { get; set; }

        public ResourceManager ResourceManager { get; set; }

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
        /// The <see cref="Member"/> value for the extension is null at the time of evaluation.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// Some part of the <see cref="Member"/> string did not parse properly-or-<paramref name="serviceProvider"/> did not provide a service for <see cref="T:System.Windows.Markup.IXamlTypeResolver"/>-or-<see cref="Member"/> value did not resolve to a static member.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="serviceProvider"/> is null.
        /// </exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.ResourceManager != null)
            {
                return CreateBindingExpression(this.ResourceManager, this.Member, serviceProvider);
            }

            try
            {
                if (string.IsNullOrEmpty(this.Member))
                {
                    throw new InvalidOperationException("MarkupExtensionStaticMember");
                }

                var qnk = QualifiedNameAndKey.Parse(this.Member);
                if (qnk.QualifiedName == null || qnk.Key == null)
                {
                    return string.Format(Resources.UnknownErrorFormat, this.Member);
                }

                var type = serviceProvider.Resolve(qnk.QualifiedName);
                if (type == null)
                {
                    return string.Format(Resources.MissingResourcesFormat, this.Member);
                }

                this.ResourceManager = ResourceManagers.ForType(type);
                this.Member = qnk.Key;
                return CreateBindingExpression(this.ResourceManager, this.Member, serviceProvider);
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
