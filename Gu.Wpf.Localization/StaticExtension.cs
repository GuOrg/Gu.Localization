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
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Reflection;
    using System.Resources;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Localization;
    using Gu.Localization.Properties;

    /// <summary>
    /// Implements a markup extension that returns static field and property references.
    /// </summary>
    [MarkupExtensionReturnType(typeof(string))]
    [ContentProperty("Member"), DefaultProperty("Member")]
    [TypeConverter(typeof(StaticExtensionConverter))]
    public class StaticExtension : System.Windows.Markup.StaticExtension
    {
        /// <summary>
        /// The cache.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ResourceManagerWrapper> Cache = new ConcurrentDictionary<Type, ResourceManagerWrapper>();

        /// <summary>
        /// The _xaml type resolver.
        /// </summary>
        private IXamlTypeResolver _xamlTypeResolver;

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
            : base(member)
        {
            Member = member;
        }

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
        /// The <paramref name="member"/> value for the extension is null at the time of evaluation.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// Some part of the <paramref name="member"/> string did not parse properly-or-<paramref name="serviceProvider"/> did not provide a service for <see cref="T:System.Windows.Markup.IXamlTypeResolver"/>-or-<paramref name="member"/> value did not resolve to a static member.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="serviceProvider"/> is null.
        /// </exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                if (Member == null)
                {
                    throw new InvalidOperationException("MarkupExtensionStaticMember");
                }

                if (DesignMode.IsDesignMode && IsTemplate(serviceProvider))
                {
                    _xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
                    return this;
                }

                var type = MemberType;
                string key;
                if (type != (Type)null)
                {
                    key = Member;
                }
                else
                {
                    var length = Member.IndexOf('.');
                    if (length < 0)
                    {
                        if (DesignMode.IsDesignMode)
                        {
                            throw new ArgumentException("Expecting format p:Resources.Key was:" + Member);
                        }

                        return string.Format(Resources.UnknownErrorFormat, Member);
                    }

                    var qualifiedTypeName = Member.Substring(0, length);
                    if (string.IsNullOrEmpty(qualifiedTypeName))
                    {
                        if (DesignMode.IsDesignMode)
                        {
                            throw new ArgumentException("Expecting format p:Resources.Key was:" + Member);
                        }

                        return string.Format(Resources.UnknownErrorFormat, Member);
                    }

                    type = GetMemberType(serviceProvider, qualifiedTypeName);

                    key = Member.Substring(length + 1, Member.Length - length - 1);
                    if (string.IsNullOrEmpty(key))
                    {
                        if (DesignMode.IsDesignMode)
                        {
                            throw new ArgumentException("Expecting format p:Resources.Key was:" + Member);
                        }

                        return string.Format(Resources.UnknownErrorFormat, Member);
                    }
                }

                var resourceManager = Cache.GetOrAdd(type, GetManager);
                if (resourceManager == null)
                {
                    if (DesignMode.IsDesignMode)
                    {
                        throw new ArgumentException("Expecting format p:Resources.Key was:" + Member);
                    }

                    return string.Format(Resources.NullManagerFormat, key);
                }

                var translation = new Translation(resourceManager, key);
                var binding = new Binding("Value")
                {
                    Source = translation
                };
                var provideValue = binding.ProvideValue(serviceProvider);
                return provideValue;
            }
            catch (Exception exception)
            {
                if (DesignMode.IsDesignMode)
                {
                    if (exception is XamlParseException)
                    {
                        return Member;
                    }
                    else
                    {
                        throw;
                    }
                }

                return string.Format(Resources.UnknownErrorFormat, Member);
            }
        }

        /// <summary>
        /// The get manager.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="ResourceManagerWrapper"/>.
        /// </returns>
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

        /// <summary>
        /// The is template.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsTemplate(IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            return target != null &&
                   !(target.TargetObject is DependencyObject);
        }

        /// <summary>
        /// The get member type.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <param name="qualifiedTypeName">
        /// The qualified type name.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        private Type GetMemberType(IServiceProvider serviceProvider, string qualifiedTypeName)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            if (_xamlTypeResolver == null)
            {
                _xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
            }

            if (_xamlTypeResolver == null)
            {
                throw new ArgumentException("MarkupExtensionNoContext IXamlTypeResolver");
            }

            return _xamlTypeResolver.Resolve(qualifiedTypeName);
        }
    }
}
