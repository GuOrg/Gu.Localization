using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gu.Wpf.Localization
{
    using System.ComponentModel;
    using System.Windows.Markup;
    using System.Xaml;

    public static class ServiceProviderExt
    {
        public static IServiceProvider ServiceProvider(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IServiceProvider>();
        }

        public static IRootObjectProvider RootObjectProvider(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IRootObjectProvider>();
        }

        public static IProvideValueTarget ProvideValueTarget(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IProvideValueTarget>();
        }

        public static IUriContext UriContext(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IUriContext>();
        }

        public static ITypeDescriptorContext TypeDescriptorContext(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<ITypeDescriptorContext>();
        }

        public static T GetService<T>(this IServiceProvider serviceProvider) where T : class
        {
            return serviceProvider.GetService(typeof (T)) as T;
        }
    }
}
