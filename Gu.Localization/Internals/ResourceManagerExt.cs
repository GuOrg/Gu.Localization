namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    internal static class ResourceManagerExt
    {
        /// <summary>
        /// Check if the <paramref name="resourceManager"/> has a translation for <paramref name="key"/>
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/></param>
        /// <param name="key">The key</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="createIfNotExits">true to load the resource set, if it has not been loaded yet; otherwise, false.</param>
        /// <returns>True if a translation exists</returns>
        internal static bool HasKey(
            this ResourceManager resourceManager,
            string key,
            CultureInfo culture,
            bool createIfNotExits)
        {
            if (createIfNotExits)
            {
                var tempManager = resourceManager.Clone();
                using (var resourceSet = tempManager.GetResourceSet(culture, true, false))
                {
                    var result = resourceSet?.OfType<DictionaryEntry>()
                                .Any(x => Equals(x.Key, key)) == true;
                    tempManager.ReleaseAllResources(); // don't think there is a way around this
                    return result;
                }
            }

            return resourceManager.GetResourceSet(culture, false, false)
                                  ?.OfType<DictionaryEntry>()
                                  .Any(x => Equals(x.Key, key)) == true;
        }

        internal static bool HasCulture(this ResourceManager resourceManager, CultureInfo culture)
        {
            if (resourceManager.GetResourceSet(culture, false, false) != null)
            {
                return true;
            }

            var tempManager = resourceManager.Clone();
            using (var resourceSet = tempManager.GetResourceSet(culture, true, false))
            {
                tempManager.ReleaseAllResources(); // don't think there is a way around this
                return resourceSet != null;
            }
        }

        private static ResourceManager Clone(this ResourceManager resourceManager)
        {
            var type = Type.ReflectionOnlyGetType(resourceManager.BaseName, true, false);
            var assembly = Assembly.GetAssembly(type);
            return new ResourceManager(resourceManager.BaseName, assembly, resourceManager.ResourceSetType);
        }
    }
}
