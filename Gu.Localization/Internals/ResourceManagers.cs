namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    /// <summary>A cache for resource managers.</summary>
    internal static class ResourceManagers
    {
        /// <summary>Tries to get from cache or create a <see cref="ResourceManager"/> for <paramref name="resourcesType"/>. </summary>
        /// <param name="resourcesType">Ex. typeof(Properties.Resources).</param>
        /// <param name="result">The <see cref="ResourceManager"/>.</param>
        /// <returns>True if a <see cref="ResourceManager"/> could be created for <paramref name="resourcesType"/>.</returns>
        internal static bool TryGetForType(Type resourcesType, [NotNullWhen(true)] out ResourceManager? result)
        {
            result = TypeManagerCache.GetOrAdd(resourcesType, x => CreateManagerForTypeOrDefault(x));
            return result != null;
        }

        /// <summary>Call with typeof(Properties.Resources).</summary>
        /// <param name="resourcesType">typeof(Properties.Resources).</param>
        /// <returns>A resource manager.</returns>
        internal static ResourceManager ForType(Type resourcesType)
        {
            var resourceManager = TypeManagerCache.GetOrAdd(resourcesType, CreateManagerForType);
            if (resourceManager is null)
            {
                var message = $"{resourcesType.FullName} must have a property named ResourceManager of type ResourceManager";
                throw new ArgumentException(message);
            }

            return resourceManager;
        }

        private static ResourceManager? CreateManagerForTypeOrDefault(Type type)
        {
            var property = GetResourceManagerProperty(type);
            return property?.GetValue(null) as ResourceManager;
        }

        private static ResourceManager CreateManagerForType(Type type)
        {
            var property = GetResourceManagerProperty(type);
            if (property is null || !typeof(ResourceManager).IsAssignableFrom(property.PropertyType))
            {
                var message = $"{type.FullName} must have a property named ResourceManager of type ResourceManager";
                throw new ArgumentException(message);
            }

            return (ResourceManager)property.GetValue(null)!;
        }

        private static PropertyInfo? GetResourceManagerProperty(this Type type)
        {
            return type.GetProperty(
                nameof(Properties.Resources.ResourceManager),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        internal static class TypeManagerCache
        {
            private static readonly ConcurrentDictionary<Type, ResourceManager?> TypeManagerMap = new ConcurrentDictionary<Type, ResourceManager?>();
            private static readonly ConcurrentDictionary<ResourceManager, Type?> ManagerTypeMap = new ConcurrentDictionary<ResourceManager, Type?>(ResourceManagerComparer.ByBaseName);

            internal static ResourceManager? GetOrAdd(Type type, Func<Type, ResourceManager?> create)
            {
                var manager = TypeManagerMap.GetOrAdd(type, create);
                if (manager != null)
                {
                    ManagerTypeMap.TryAdd(manager, type);
                }

                return manager;
            }

            internal static Type? GetOrAdd(ResourceManager resourceManager)
            {
                var type = ManagerTypeMap.GetOrAdd(resourceManager, x => ContainingType(x));
                if (type != null)
                {
                    TypeManagerMap.TryAdd(type, resourceManager);
                }

                return type;
            }

            private static Type? ContainingType(ResourceManager resourceManager)
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                                .Select(x => x.GetType(resourceManager.BaseName))
                                .SingleOrDefault(x => x != null &&
                                                      x.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                                       .Any(p => p.PropertyType == typeof(ResourceManager)));
            }
        }
    }
}
