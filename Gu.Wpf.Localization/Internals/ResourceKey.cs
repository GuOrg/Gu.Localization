namespace Gu.Wpf.Localization.Internals
{
    using System;
    using System.Resources;
    using System.Windows.Markup;

    using Gu.Localization;

    internal class ResourceKey
    {
        public ResourceKey(ResourceManager resourceManager, string key)
        {
            ResourceManager = resourceManager;
            Key = key;
            HasError = string.IsNullOrEmpty(Key) || ResourceManager == null;
        }

        public ResourceKey(ResourceManager resourceManager, string key, bool hasError)
        {
            ResourceManager = resourceManager;
            Key = key;
            HasError = hasError;
        }

        public bool HasError { get; private set; }

        public ResourceManager ResourceManager { get; private set; }

        public string Key { get; private set; }

        public static ResourceKey Resolve(string member, IXamlTypeResolver typeResolver, bool throwOnError)
        {
            TypeNameAndKey typeNameAndKey;
            if (!TypeNameAndKey.TryParse(member, out typeNameAndKey))
            {
                if (throwOnError)
                {
                    throw new ArgumentException("Expecting format p:Resources.Key was:" + member);
                }
                return new ResourceKey(null, null, true);
            }
            var type = typeResolver.Resolve(typeNameAndKey.QualifiedTypeName);
            var manager = ResourceManagerWrapper.FromType(type);
            return new ResourceKey(manager, typeNameAndKey.Key);
        }
    }
}