namespace Gu.Wpf.Localization
{
    using System;
    using System.Globalization;
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
        private static readonly PropertyPath TranslatedPropertyPath = new(nameof(Gu.Localization.Translation.Translated));

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
        /// <exception cref="ArgumentNullException">
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
                if (qnk.QualifiedName is null || qnk.Key is null)
                {
                    return string.Format(CultureInfo.InvariantCulture, Resources.UnknownErrorFormat, this.Member);
                }

                var type = serviceProvider.Resolve(qnk.QualifiedName);
                if (type is null)
                {
                    return string.Format(CultureInfo.InvariantCulture, Resources.MissingResourcesFormat, this.Member);
                }

                var manager = Gu.Localization.ResourceManagers.ForType(type);
                this.Member = qnk.Key;
#pragma warning disable CA1304 // Specify CultureInfo
                if (!manager.HasKey(qnk.Key))
#pragma warning restore CA1304 // Specify CultureInfo
                {
                    return string.Format(CultureInfo.InvariantCulture, Resources.MissingKeyFormat, this.Member);
                }

                return CreateBindingExpression(manager, this.Member, serviceProvider);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return string.Format(CultureInfo.InvariantCulture, Resources.UnknownErrorFormat, this.Member);
            }
        }

        internal static object CreateBindingExpression(ResourceManager resourceManager, string key, IServiceProvider serviceProvider)
        {
            switch (serviceProvider.ProvideValueTarget())
            {
                case { TargetObject: Binding }:
                    return Translation.GetOrCreate(resourceManager, key, GetErrorHandling(serviceProvider.RootObjectProvider()?.RootObject));
                case { TargetObject: { } o }:
                    var translation = Translation.GetOrCreate(resourceManager, key, GetErrorHandling(o));
                    var binding = new Binding
                    {
                        Path = TranslatedPropertyPath,
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Source = translation,
                    };

                    var provideValue = binding.ProvideValue(serviceProvider);
                    return provideValue;
                default:
                    return Translation.GetOrCreate(resourceManager, key, GetErrorHandling(serviceProvider.RootObjectProvider()?.RootObject));
            }

            static Gu.Localization.ErrorHandling GetErrorHandling(object? o)
            {
                if (o is DependencyObject dependencyObject &&
                    ErrorHandling.GetMode(dependencyObject) is { } errorHandling)
                {
                    return errorHandling;
                }

                return Gu.Localization.ErrorHandling.ReturnErrorInfoPreserveNeutral;
            }
        }
    }
}
