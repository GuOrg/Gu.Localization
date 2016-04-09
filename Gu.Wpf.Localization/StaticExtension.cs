// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticExtension.cs" company="">
//
// </copyright>
// <summary>
//   Implements a markup extension that returns static field and property references.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Resources;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Localization;
    using Gu.Localization.Properties;

    /// <summary>
    /// Implements a markup extension that translates resources.
    /// The reason for the name StaticExtension is that it tricks Resharper into providing Intellisense.
    /// l:Static p:Resources.YourKey
    /// </summary>
    [MarkupExtensionReturnType(typeof(string))]
    [ContentProperty("Member")]
    [DefaultProperty("Member")]
    [TypeConverter(typeof(StaticExtensionConverter))]
    public class StaticExtension : MarkupExtension
    {
        /// <summary>
        /// The _xaml type resolver.
        /// </summary>
        private IXamlTypeResolver xamlTypeResolver;

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
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (string.IsNullOrEmpty(this.Member))
            {
                throw new InvalidOperationException("MarkupExtensionStaticMember");
            }

            try
            {
                if (DesignMode.IsDesignMode && IsInTemplate(serviceProvider))
                {
                    this.xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
                    return this;
                }

                if (this.xamlTypeResolver == null)
                {
                    this.xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
                }

                var resourceKey = new ResourceKey(this.Member, this.xamlTypeResolver, DesignMode.IsDesignMode);
                if (resourceKey.HasError)
                {
                    return string.Format(Resources.UnknownErrorFormat, this.Member);
                }

                var translation = new Translation(resourceKey.ResourceManager, resourceKey.Key);
                var binding = new Binding(ExpressionHelper.PropertyName(() => translation.Translated))
                {
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = translation
                };
                var provideValue = binding.ProvideValue(serviceProvider);
                return provideValue;
            }
            catch (Exception exception)
            {
                ////if (DesignMode.IsDesignMode)
                ////{
                ////    if (exception is XamlParseException)
                ////    {
                ////        return Member;
                ////    }
                ////    else
                ////    {
                ////        throw;
                ////    }
                ////}

                return string.Format(Resources.UnknownErrorFormat, this.Member);
            }
        }

        private static bool IsInTemplate(IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            return target != null && !(target.TargetObject is DependencyObject);
        }
    }
}
