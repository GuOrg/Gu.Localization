namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Resources;

    internal sealed class ResourceManagerComparer : IEqualityComparer<ResourceManager>
    {
        internal static readonly ResourceManagerComparer ByBaseName = new();
        private static readonly StringComparer StringComparer = StringComparer.Ordinal;

        private ResourceManagerComparer()
        {
        }

        public bool Equals(ResourceManager? x, ResourceManager? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return StringComparer.Equals(x.BaseName, y.BaseName);
        }

        public int GetHashCode(ResourceManager obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return StringComparer.GetHashCode(obj.BaseName);
        }
    }
}
