namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// Implements a markup extension that returns static field and property references.
    /// </summary>
    [MarkupExtensionReturnType(typeof(string))]
    [TypeConverter(typeof(ResourceExtensionConverter))]
    //[TypeForwardedFrom("PresentationFramework, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
    public class ResourceExtension : StaticExtension
    {
        private static readonly ConcurrentDictionary<Type, ResourceManagerWrapper> Cache = new ConcurrentDictionary<Type, ResourceManagerWrapper>();
        private IXamlTypeResolver xamlTypeResolver;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Gu.Wpf.Localization.ResourceExtension"/> class using the provided <paramref name="member"/> string.
        /// </summary>
        /// <param name="member">A string that identifies the member to make a reference to. This string uses the format prefix:typeName.fieldOrPropertyName. prefix is the mapping prefix for a XAML namespace, and is only required to reference static values that are not mapped to the default XAML namespace.</param><exception cref="T:System.ArgumentNullException"><paramref name="member"/> is null.</exception>
        public ResourceExtension(string member)
            : base(member)
        {
        }

        /// <summary>
        /// Returns an object value to set on the property where you apply this extension. For <see cref="T:System.Windows.Markup.ResourceExtension"/>, the return value is the static value that is evaluated for the requested static member.
        /// </summary>
        /// 
        /// <returns>
        /// The static value to set on the property where the extension is applied.
        /// </returns>
        /// <param name="serviceProvider">An object that can provide services for the markup extension. The service provider is expected to provide a service that implements a type resolver (<see cref="T:System.Windows.Markup.IXamlTypeResolver"/>).</param><exception cref="T:System.InvalidOperationException">The <paramref name="member"/> value for the extension is null at the time of evaluation.</exception><exception cref="T:System.ArgumentException">Some part of the <paramref name="member"/> string did not parse properly-or-<paramref name="serviceProvider"/> did not provide a service for <see cref="T:System.Windows.Markup.IXamlTypeResolver"/>-or-<paramref name="member"/> value did not resolve to a static member.</exception><exception cref="T:System.ArgumentNullException"><paramref name="serviceProvider"/> is null.</exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.Member == null)
                throw new InvalidOperationException("MarkupExtensionStaticMember");
            if (DesignMode.IsDesignMode)
            {
                var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
                if (target != null && target.TargetObject is FrameworkElement)
                {
                    var frameworkElement = (FrameworkElement)target.TargetObject;
                    var value = (XmlLanguage)frameworkElement.GetValue(FrameworkElement.LanguageProperty);
                    if (value != null)
                    {
                        Translator.CurrentCulture = CultureInfo.GetCultureInfo(value.IetfLanguageTag);
                    }
                }
                if (target != null && !(target.TargetObject is DependencyObject))
                {
                    xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;

                    return this;
                }
            }

            Type type = this.MemberType;
            string name;
            string str;
            if (type != (Type)null)
            {
                name = this.Member;
                str = type.FullName + "." + this.Member;
            }
            else
            {
                str = this.Member;
                int length = this.Member.IndexOf('.');
                if (length < 0)
                {
                    if (DesignMode.IsDesignMode)
                    {
                        throw new ArgumentException("Expecting format p:Resources.Key was:" + Member);
                    }
                    return string.Format(Properties.Resources.UnknownErrorFormat, Member);
                }
                else
                {
                    string qualifiedTypeName = this.Member.Substring(0, length);
                    if (qualifiedTypeName == string.Empty)
                    {
                        if (DesignMode.IsDesignMode)
                        {
                            throw new ArgumentException("Expecting format p:Resources.Key was:" + Member);
                        }
                        return string.Format(Properties.Resources.UnknownErrorFormat, Member);
                    }
                    else
                    {
                        if (serviceProvider == null)
                            throw new ArgumentNullException("serviceProvider");
                        if (xamlTypeResolver == null)
                        {
                            xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
                        }
                        if (xamlTypeResolver == null)
                        {
                            throw new ArgumentException("MarkupExtensionNoContext IXamlTypeResolver");
                        }
                        else
                        {
                            type = xamlTypeResolver.Resolve(qualifiedTypeName);
                            name = this.Member.Substring(length + 1, this.Member.Length - length - 1);
                            if (name == string.Empty)
                            {
                                if (DesignMode.IsDesignMode)
                                {
                                    throw new ArgumentException("Expecting format p:Resources.Key was:" + Member);
                                }
                                return string.Format(Properties.Resources.UnknownErrorFormat, Member);
                            }
                        }
                    }
                }
            }

            var resourceManager = Cache.GetOrAdd(type, GetManager);
            if (resourceManager == null)
            {
                if (DesignMode.IsDesignMode)
                {
                    throw new ArgumentException("Expecting format p:Resources.Key was:" + Member);
                }
                return string.Format(Properties.Resources.NullManagerFormat, name);
            }

            var translator = new Translator(resourceManager, name);
            var binding = new Binding("Value")
            {
                Source = translator
            };
            var provideValue = binding.ProvideValue(serviceProvider);
            return provideValue;
        }

        private static ResourceManagerWrapper GetManager(Type type)
        {
            var propertyInfo = type.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var resourceManager = propertyInfo.GetValue(null) as ResourceManager;
            if (resourceManager == null)
            {
                return null;
            }
            return new ResourceManagerWrapper(resourceManager);
        }
    }
}
