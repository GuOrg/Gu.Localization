namespace Gu.Wpf.Localization.Internals
{
    using System;
    using System.Diagnostics;
    using System.Windows.Markup;

    public static class ServiceProviderExt
    {
        public static IXamlTypeResolver GetXamlTypeResolver(this IServiceProvider provider)
        {
            return provider.GetService<IXamlTypeResolver>();
        }

        public static Type Resolve(this IServiceProvider serviceProvider, string qualifiedTypeName)
        {
            var xamlTypeResolver = serviceProvider.GetXamlTypeResolver();
            if (xamlTypeResolver == null && Designtime.DesignMode.IsDesignMode)
            {
                Debugger.Break();
            }
            return xamlTypeResolver.Resolve(qualifiedTypeName);
        }

        internal static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }
    }
}
