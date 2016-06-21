namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Resources;

    internal class ResourceManagerComparer : IEqualityComparer<ResourceManager>
    {
        public static readonly ResourceManagerComparer ByBaseName = new ResourceManagerComparer();
        private static readonly StringComparer StringComparer = StringComparer.Ordinal;

        private ResourceManagerComparer()
        {
        }

        public bool Equals(ResourceManager x, ResourceManager y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return StringComparer.Equals(x.BaseName, y.BaseName);
        }

        public int GetHashCode(ResourceManager obj)
        {
            Ensure.NotNull(obj, nameof(obj));
            return StringComparer.GetHashCode(obj.BaseName);
        }
    }
}