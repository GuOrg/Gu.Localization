namespace Gu.Wpf.Localization
{
    using System;
    using System.Resources;
    using System.Text.RegularExpressions;
    using System.Windows.Markup;

    using Gu.Localization;
    using Gu.Localization.Properties;

    internal class ResourceKey
    {
        public ResourceKey(string member, IXamlTypeResolver typeResolver, bool throwOnError)
        {
            var match = Regex.Match(member, @"(?<ns>\w+):(?<resources>\w+)\.(?<key>\w+)");
            if (!match.Success)
            {
                HasError = true;
                if (throwOnError)
                {
                    throw new ArgumentException("Expecting format p:Resources.Key was:" + member);
                }
                return;
            }

            var qualifiedTypeName = string.Format("{0}:{1}", match.Groups["ns"].Value, match.Groups["resources"].Value);
            var type = typeResolver.Resolve(qualifiedTypeName);
            ResourceManager = ResourceManagerWrapper.FromType(type);

            Key = match.Groups["key"].Value;
            HasError = string.IsNullOrEmpty(Key) || ResourceManager == null;
        }

        public bool HasError { get; private set; }

        public ResourceManager ResourceManager { get; private set; }

        public string Key { get; private set; }
    }
}