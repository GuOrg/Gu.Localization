namespace Gu.Wpf.Localization.Internals
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Markup;
    using Gu.Wpf.Localization.Designtime;

    internal static class ServiceProviderExt
    {
        internal static IXamlTypeResolver GetXamlTypeResolver(this IServiceProvider provider)
        {
            return provider.GetService<IXamlTypeResolver>();
        }

        internal static Type Resolve(this IServiceProvider serviceProvider, string qualifiedTypeName)
        {
            var xamlTypeResolver = serviceProvider.GetXamlTypeResolver();
            if (xamlTypeResolver == null && Design.IsDesignMode)
            {
                Debugger.Break();
            }
            return xamlTypeResolver.Resolve(qualifiedTypeName);
        }

        internal static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }

        internal static bool IsInTemplate(this IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            return target != null && !(target.TargetObject is DependencyObject);
        }
    }
}
