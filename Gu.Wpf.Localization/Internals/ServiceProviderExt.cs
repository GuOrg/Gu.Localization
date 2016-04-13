namespace Gu.Wpf.Localization
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;

    internal static class ServiceProviderExt
    {
        internal static IXamlTypeResolver GetXamlTypeResolver(this IServiceProvider provider)
        {
            return provider.GetService<IXamlTypeResolver>();
        }

        internal static Type Resolve(this IServiceProvider serviceProvider, string qualifiedTypeName)
        {
            var xamlTypeResolver = serviceProvider.GetXamlTypeResolver();
            if (xamlTypeResolver == null)
            {
                if (Is.DesignMode)
                {
                    Debugger.Break();
                }

                return null;
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