namespace Gu.Localization
{
    using System;
    using System.Resources;

    /// <summary> Split up nested class.  </summary>
    public partial class Translation
    {
        private struct ResourceManagerAndKey : IEquatable<ResourceManagerAndKey>
        {
            internal readonly ResourceManager ResourceManager;
            internal readonly string Key;
            private readonly ErrorHandling errorHandling;

            internal ResourceManagerAndKey(ResourceManager resourceManager, string key, ErrorHandling errorHandling)
            {
                this.ResourceManager = resourceManager;
                this.Key = key;
                this.errorHandling = errorHandling;
            }

            public bool Equals(ResourceManagerAndKey other)
            {
                return this.ResourceManager.Equals(other.ResourceManager) &&
                       string.Equals(this.Key, other.Key, StringComparison.Ordinal) &&
                       this.errorHandling == other.errorHandling;
            }

            public override bool Equals(object obj)
            {
                return obj is ResourceManagerAndKey key &&
                       this.Equals(key);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = this.ResourceManager.GetHashCode();
                    hashCode = (hashCode * 397) ^ StringComparer.InvariantCulture.GetHashCode(this.Key);
                    hashCode = (hashCode * 397) ^ (int)this.errorHandling;
                    return hashCode;
                }
            }
        }
    }
}
