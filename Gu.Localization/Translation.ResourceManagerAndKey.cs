namespace Gu.Localization
{
    using System;
    using System.Resources;

    public partial class Translation
    {
        private struct ResourceManagerAndKey : IEquatable<ResourceManagerAndKey>
        {
            internal readonly ResourceManager ResourceManager;
            internal readonly string Key;

            public ResourceManagerAndKey(ResourceManager resourceManager, string key)
            {
                this.ResourceManager = resourceManager;
                this.Key = key;
            }

            public bool Equals(ResourceManagerAndKey other)
            {
                return string.Equals(this.ResourceManager?.BaseName, other.ResourceManager?.BaseName) && string.Equals(this.Key, other.Key);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                return obj is ResourceManagerAndKey && this.Equals((ResourceManagerAndKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((this.ResourceManager?.BaseName?.GetHashCode() ?? 0) * 397) ^ (this.Key?.GetHashCode() ?? 0);
                }
            }
        }
    }
}
