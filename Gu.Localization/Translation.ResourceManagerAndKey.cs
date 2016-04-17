namespace Gu.Localization
{
    using System;
    using System.Diagnostics;
    using System.Resources;

    /// <summary> Split up nested class  </summary>
    public partial class Translation
    {
        private struct ResourceManagerAndKey : IEquatable<ResourceManagerAndKey>
        {
            internal readonly ResourceManager ResourceManager;
            internal readonly string Key;
            private readonly ErrorHandling errorHandling;

            public ResourceManagerAndKey(ResourceManager resourceManager, string key, ErrorHandling errorHandling)
            {
                Ensure.NotNull(resourceManager, nameof(resourceManager));
                Ensure.NotNull(key, nameof(key));
                this.ResourceManager = resourceManager;
                this.Key = key;
                this.errorHandling = errorHandling;
            }

            public bool Equals(ResourceManagerAndKey other)
            {
                return this.ResourceManager.Equals(other.ResourceManager) &&
                       string.Equals(this.Key, other.Key) &&
                       this.errorHandling == other.errorHandling;
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
                    var hashCode = this.ResourceManager.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Key.GetHashCode();
                    hashCode = (hashCode * 397) ^ (int)this.errorHandling;
                    return hashCode;
                }
            }
        }
    }
}
