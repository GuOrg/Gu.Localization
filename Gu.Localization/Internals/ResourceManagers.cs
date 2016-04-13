namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Resources;

    internal static class ResourceManagers
    {
        private static readonly ConcurrentDictionary<Type, ResourceManager> TypeManagerMap = new ConcurrentDictionary<Type, ResourceManager>();

        internal static bool TryGetForType(Type resourcesType, out ResourceManager result)
        {
            result = TypeManagerMap.GetOrAdd(resourcesType, CreateManagerForType);
            return result != null;
        }

        /// <summary>
        /// Call with typeof(Properties.Resources)
        /// </summary>
        /// <param name="resourcesType">typeof(Properties.Resources)</param>
        /// <returns>A resource manager</returns>
        internal static ResourceManager ForType(Type resourcesType)
        {
            var resourceManager = TypeManagerMap.GetOrAdd(resourcesType, CreateManagerForType);
            if (resourceManager == null)
            {
                var message = $"{nameof(resourcesType)} must have a property named ResourceManager of type ResourceManager";
                throw new InvalidOperationException(message);
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