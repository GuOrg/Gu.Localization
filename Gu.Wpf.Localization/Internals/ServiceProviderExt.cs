namespace Gu.Wpf.Localization.Internals
{
    using System;
    using System.Windows.Markup;

    public static class ServiceProviderExt
    {
        public static IXamlTypeResolver GetXamlTypeResolver(this IServiceProvider provider)
        {
            return provider.GetService<IXamlTypeResolver>();
        }

        internal static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }
    }
}
