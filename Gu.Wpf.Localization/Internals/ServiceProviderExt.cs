namespace Gu.Wpf.Localization.Internals
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Markup;
    using System.Xaml;

    internal static class ServiceProviderExt
    {
        internal static IXamlTypeResolver XamlTypeResolver(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
        }

        internal static IProvideValueTarget ProvideValueTarget(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        }

        internal static IRootObjectProvider RootObjectProvider(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
        }

        internal static ITypeDescriptorContext TypeDescriptorContext(this IServiceProvider provider)
        {
            return provider.GetService(typeof(ITypeDescriptorContext)) as ITypeDescriptorContext;
        }

        internal static IUriContext UriContext(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IUriContext)) as IUriContext;
        }

        internal static IXamlNameResolver XamlNameResolver(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlNameResolver)) as IXamlNameResolver;
        }

        internal static IXamlNamespaceResolver XamlNamespaceResolver(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlNamespaceResolver)) as IXamlNamespaceResolver;
        }

        internal static IXamlSchemaContextProvider XamlSchemaContextProvider(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
        }

        internal static bool IsInTemplate(this IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            return target != null && !(target.TargetObject is DependencyObject);
        }
    }
}
