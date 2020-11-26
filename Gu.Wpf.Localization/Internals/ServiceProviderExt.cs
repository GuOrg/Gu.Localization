#pragma warning disable SA1600 // Elements must be documented

namespace Gu.Wpf.Localization
{
    using System;
    using System.Windows.Markup;
    using System.Xaml;

    internal static class ServiceProviderExt
    {
        internal static IProvideValueTarget ProvideValueTarget(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IProvideValueTarget>();
        }

        internal static IRootObjectProvider? RootObjectProvider(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IRootObjectProvider?>();
        }

        internal static Type? Resolve(this IServiceProvider serviceProvider, string qualifiedTypeName)
        {
            var xamlTypeResolver = serviceProvider.GetXamlTypeResolver();
            return xamlTypeResolver?.Resolve(qualifiedTypeName);
        }

        private static IXamlTypeResolver GetXamlTypeResolver(this IServiceProvider provider)
        {
            return provider.GetService<IXamlTypeResolver>();
        }

        private static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }
    }
}
