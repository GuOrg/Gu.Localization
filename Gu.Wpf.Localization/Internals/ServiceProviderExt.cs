namespace Gu.Wpf.Localization
{
    using System;
    using System.Windows.Markup;
    using System.Xaml;

    internal static class ServiceProviderExt
    {
        internal static IProvideValueTarget ProvideValueTarget(this IServiceProvider serviceProvider) => serviceProvider.GetService<IProvideValueTarget>() ?? throw new InvalidOperationException("GetService<IProvideValueTarget>() returned null");

        internal static IRootObjectProvider? RootObjectProvider(this IServiceProvider serviceProvider) => serviceProvider.GetService<IRootObjectProvider>();

        internal static Type? Resolve(this IServiceProvider serviceProvider, string qualifiedTypeName)
        {
            if (serviceProvider.GetService<IXamlTypeResolver>() is { } xamlTypeResolver)
            {
                return xamlTypeResolver.Resolve(qualifiedTypeName);
            }

            return null;
        }

        private static T? GetService<T>(this IServiceProvider provider) => (T?)provider.GetService(typeof(T));
    }
}
