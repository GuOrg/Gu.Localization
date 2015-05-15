namespace Gu.Wpf.Localization.Internals
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Windows.Data;
    using System.Windows.Markup;
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
                return "could not parse: " + member;
            }
            return DumpProviders(serviceProvider, typeNameAndKey);
            //ResourceKey resourceKey;
            //if (TryUsingXamlTypeResolver(serviceProvider, typeNameAndKey, out resourceKey))
            //{
            //    return ProvideValue(resourceKey, serviceProvider);
            //}

            //if (TryUsingXamlSchemaContext(serviceProvider, typeNameAndKey, out resourceKey))
            //{
            //    return ProvideValue(resourceKey, serviceProvider);
            //}
            //return null;
        }

        private static object DumpProviders(IServiceProvider serviceProvider, TypeNameAndKey typeNameAndKey)
        {
            var sb = new StringBuilder();
            //Dump<IProvideValueTarget>(serviceProvider, sb, x => x.TargetProperty.ToString());
            //Dump<IRootObjectProvider>(serviceProvider, sb, x => x.RootObject.ToString());
            //Dump<IUriContext>(serviceProvider, sb, x => x.BaseUri.ToString());

            //Dump<IXamlNameProvider>(serviceProvider, sb, null);
            Dump<IXamlNameResolver>(serviceProvider, sb, null);
            var xamlNameResolver = serviceProvider.XamlNameResolver();
            sb.AppendLine(xamlNameResolver.IsFixupTokenAvailable.ToString());
            Dump<IXamlNamespaceResolver>(serviceProvider, sb, x => x.GetNamespace(typeNameAndKey.Prefix));
            Dump<IXamlSchemaContextProvider>(serviceProvider, sb, null);

            //try
            //{
            //    var xamlTypeName = XamlTypeName.Parse(typeNameAndKey.QualifiedTypeName, (IXamlNamespaceResolver)serviceProvider);
            //    sb.AppendFormat("xamltypename: {0}.{1}" , xamlTypeName.Namespace, xamlTypeName.Name);
            //    //Dump<IXamlSchemaContextProvider>(serviceProvider, sb, x => x.SchemaContext.GetXamlType(xamlTypeName).Name);

            //}
            //catch (Exception e)
            //{
            //    sb.AppendFormat("xamltypename: {0}", e.Message);
            //    throw;
            //}
            //sb.AppendLine();
            //Dump<IXamlTypeResolver>(serviceProvider, sb, null);
            //Dump<IXamlTypeResolver>(serviceProvider, sb, x => x.Resolve(typeNameAndKey.QualifiedTypeName).Name);
            return sb.ToString();
        }

        private static void Dump<T>(IServiceProvider serviceProvider, StringBuilder sb, Func<T, string> func)
        {
            var service = (T)serviceProvider.GetService(typeof(T));
            sb.AppendFormat("{0}: {1}", typeof(T).Name, service == null ? "null" : "");
            if (service != null && func != null)
            {
                try
                {
                    sb.AppendFormat(": {0}", func(service));
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
            }
            sb.AppendLine();
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
