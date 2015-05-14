namespace Gu.Wpf.Localization.Internals
{
    using System;
    using System.Linq;
    using System.Windows.Data;
    using System.Xaml;
    using System.Xaml.Schema;

    using Gu.Localization;

    internal static class DesigntimeSandbox
    {
        public static object ProvideValue(IServiceProvider serviceProvider, string member)
        {
            TypeNameAndKey typeNameAndKey;
            if (!TypeNameAndKey.TryParse(member, out typeNameAndKey))
            {
                return new ResourceKey(null, "could not parse: " + member);
            }
            ResourceKey resourceKey;
            if (TryUsingXamlTypeResolver(serviceProvider, typeNameAndKey, out resourceKey))
            {
                return ProvideValue(resourceKey, serviceProvider);
            }

            if (TryUsingXamlSchemaContext(serviceProvider, typeNameAndKey, out resourceKey))
            {
                return ProvideValue(resourceKey, serviceProvider);
            }
            return null;
        }

        private static object UsingXamlNamespaceResolver(IServiceProvider serviceProvider, string member)
        {
            var resolver = serviceProvider.XamlNamespaceResolver();
            if (resolver == null)
            {
                return "xamlNamespaceResolver == null";
            }
            var prefix = member.Split(':').FirstOrDefault();
            if (prefix != null)
            {
                var ns = resolver.GetNamespace(prefix);
                return ns;
            }
            return "prefix:" + prefix;
        }

        private static bool TryUsingXamlTypeResolver(IServiceProvider serviceProvider, TypeNameAndKey member, out ResourceKey result)
        {
            result = null;
            var resolver = serviceProvider.XamlTypeResolver();
            if (resolver == null)
            {
                return false;
            }

            var type = resolver.Resolve(member.QualifiedTypeName);
            if (type == null)
            {
                return false;
            }
            var manager = ResourceManagerWrapper.FromType(type);
            result = new ResourceKey(manager, member.Key, false);
            return true;
        }

        private static bool TryUsingXamlSchemaContext(IServiceProvider serviceProvider, TypeNameAndKey member, out ResourceKey result)
        {
            result = null;
            var context = serviceProvider.XamlSchemaContextProvider();
            if (context == null)
            {
                return false;
            }
            var xamlNamespaceResolver = serviceProvider.XamlNamespaceResolver();
            if (xamlNamespaceResolver == null)
            {
                return false;
            }
            var type = context.SchemaContext.GetXamlType(XamlTypeName.Parse(member.QualifiedTypeName, xamlNamespaceResolver))
                            .UnderlyingType;
            var manager = ResourceManagerWrapper.FromType(type);
            result = new ResourceKey(manager, member.Key, false);
            return true;
        }

        private static object ProvideValue(ResourceKey resourceKey, IServiceProvider serviceProvider)
        {
            var translation = new Translation(resourceKey.ResourceManager, resourceKey.Key);
            var binding = new Binding(ExpressionHelper.PropertyName(() => translation.Translated))
            {
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Source = translation
            };
            var provideValue = binding.ProvideValue(serviceProvider);
            return provideValue;
        }
    }
}
