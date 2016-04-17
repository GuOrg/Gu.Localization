namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Resources;

    /// <summary>A cache for resourcemanagers.</summary>
        internal static class ResourceManagers
    {
        private static readonly ConcurrentDictionary<Type, ResourceManager> TypeManagerMap = new ConcurrentDictionary<Type, ResourceManager>();

        /// <summary>Tries to get from cache or create a <see cref="ResourceManager"/> for <paramref name="resourcesType"/> </summary>
        /// <param name="resourcesType">Ex. typeof(Properties.Resources)</param>
        /// <param name="result">The <see cref="ResourceManager"/></param>
        /// <returns>True if a <see cref="ResourceManager"/> could be created for <paramref name="resourcesType"/></returns>
        internal static bool TryGetForType(Type resourcesType, out ResourceManager result)
        {
            result = TypeManagerMap.GetOrAdd(resourcesType, CreateManagerForType);
            return result != null;
        }

        /// <summary>Call with typeof(Properties.Resources)</summary>
        /// <param name="resourcesType">typeof(Properties.Resources)</param>
        /// <returns>A resource manager</returns>
        internal static ResourceManager ForType(Type resourcesType)
        {
            var resourceManager = TypeManagerMap.GetOrAdd(resourcesType, CreateManagerForType);
            if (resourceManager == null)
            {
                var message = $"{nameof(resourcesType)} must have a property named ResourceManager of type ResourceManager";
                throw new ArgumentException(message);
            }

            return resourceManager;
        }

        private static ResourceManager CreateManagerForType(Type type)
        {
            var property = type.GetProperty(nameof(Properties.Resources.ResourceManager), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (property == null || !typeof(ResourceManager).IsAssignableFrom(property.PropertyType))
            {
                return null;
            }

            return (ResourceManager)property.GetValue(null);
        }
    }
}