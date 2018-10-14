namespace Gu.Wpf.Localization
{
    using System;
    using System.Resources;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Localization;
    using Gu.Localization.Properties;

    /// <summary>
    /// A markup extension that translates resources.
    /// The reason for the name StaticExtension is that it tricks Resharper into providing Intellisense.
    /// Usage: Text="{l:Static p:Resources.SomeResource}".
    /// </summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class StaticExtension : System.Windows.Markup.StaticExtension
    {
        private static readonly PropertyPath TranslatedPropertyPath = new PropertyPath(nameof(Gu.Localization.Translation.Translated));

        /// <summary> Initializes a new instance of the <see cref="StaticExtension"/> class.</summary>
        public StaticExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticExtension"/> class using the provided <paramref name="member"/> string.
        /// </summary>
        /// <param name="member">
        /// A string that identifies the member to make a reference to. This string uses the format prefix:typeName.fieldOrPropertyName.
        /// prefix is the mapping prefix for a XAML namespace, and is only required to reference static values that are not mapped to the default XAML namespace.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="member"/> is null.
        /// </exception>
        public StaticExtension(string member)
            : base(member)
        {
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                if (this.MemberType != null)
                {
                    var resourceManager = Gu.Localization.ResourceManagers.ForType(this.MemberType);
                    return CreateBindingExpression(resourceManager, this.Member, serviceProvider);
                }

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

                var manager = Gu.Localization.ResourceManagers.ForType(type);
                this.Member = qnk.Key;
                if (!manager.HasKey(qnk.Key))
                {
                    return string.Format(Resources.MissingKeyFormat, this.Member);
                }

                return CreateBindingExpression(manager, this.Member, serviceProvider);
            }
            catch (Exception)
            {
                return string.Format(Resources.UnknownErrorFormat, this.Member);
            }
        }

        internal static object CreateBindingExpression(ResourceManager resourceManager, string key, IServiceProvider serviceProvider)
        {
            var errorHandling = ErrorHandling.GetMode(serviceProvider.ProvideValueTarget()?.TargetObject as DependencyObject) ??
                                              Gu.Localization.ErrorHandling.ReturnErrorInfoPreserveNeutral;
            var translation = Gu.Localization.Translation.GetOrCreate(resourceManager, key, errorHandling);
            var binding = new Binding
            {
                Path = TranslatedPropertyPath,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Source = translation,
            };

            var provideValue = binding.ProvideValue(serviceProvider);
            return provideValue;
        }
    }
}