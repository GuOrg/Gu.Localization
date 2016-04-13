namespace Gu.Wpf.Localization
{
    using System;
    using System.Resources;
    using System.Text.RegularExpressions;
    using System.Windows.Markup;

    using Gu.Localization;

    internal class ResourceKey
    {
        public ResourceKey(string member, IXamlTypeResolver typeResolver, bool throwOnError)
        {
            var match = Regex.Match(member, @"(?<ns>\w+):(?<resources>\w+)\.(?<key>\w+)");
            if (!match.Success)
            {
                this.HasError = true;
                if (throwOnError)
                {
                    throw new ArgumentException("Expecting format p:Resources.Key was:" + member);
                }

                return;
            }

            var qualifiedTypeName = $"{match.Groups["ns"].Value}:{match.Groups["resources"].Value}";
            var type = typeResolver.Resolve(qualifiedTypeName);
            this.ResourceManager = ResourceManagers.ForType(type);

            this.Key = match.Groups["key"].Value;
            this.HasError = string.IsNullOrEmpty(this.Key) || this.ResourceManager == null;
        }

        public bool HasError { get; }

        public ResourceManager ResourceManager { get; }

        public string Key { get; }
    }
}