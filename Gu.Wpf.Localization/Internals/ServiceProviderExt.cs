#pragma warning disable SA1600 // Elements must be documented

namespace Gu.Wpf.Localization
{
    using System;
    using System.Windows;
    using System.Windows.Markup;

    internal static class ServiceProviderExt
    {
        internal static IProvideValueTarget ProvideValueTarget(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IProvideValueTarget>();
        }

        internal static Type Resolve(this IServiceProvider serviceProvider, string qualifiedTypeName)
        {
            var xamlTypeResolver = serviceProvider.GetXamlTypeResolver();
            if (xamlTypeResolver == null)
            {
                ////if (Is.DesignMode)
                ////{
                ////    Debugger.Break();
                ////}

                return null;
            }

            return xamlTypeResolver.Resolve(qualifiedTypeName);
        }

        private static IXamlTypeResolver GetXamlTypeResolver(this IServiceProvider provider)
        {
            return provider.GetService<IXamlTypeResolver>();
        }

        private static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }

        private static bool IsInTemplate(this IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            return target != null && !(target.TargetObject is DependencyObject);
        }
    }
}