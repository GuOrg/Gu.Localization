namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Resources;

    [DebuggerDisplay(@"Assembly: {Assembly.GetName().Name} Languages: {string.Join("", "", Languages.Select(x=>x.TwoLetterISOLanguageName))}")]
    internal class ResourceManagerWrapper : ILanguageManager
    {
        private readonly Dictionary<CultureInfo, ResourceSet> _culturesAndResourceSets = new Dictionary<CultureInfo, ResourceSet>();
        internal ResourceManagerWrapper(ResourceManager resourceManager)
        {
            if (resourceManager == null)
            {
                throw new ArgumentNullException(nameof(resourceManager));
            }
            ResourceManager = resourceManager;
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                              .Where(x => x.Name != "")
                              .ToArray();
            foreach (var culture in cultureInfos)
            {
                var resourceSet = ResourceManager.GetResourceSet(culture, true, false);
                if (resourceSet != null)
                {
                    _culturesAndResourceSets[culture] = resourceSet;
                }
            }
            Languages = _culturesAndResourceSets.Keys.ToArray();
            Translator.AllCulturesInner.UnionWith(Languages);
        }

        public ResourceManager ResourceManager { get; }

        public IReadOnlyList<CultureInfo> Languages { get; }

        public string Translate(CultureInfo culture, string key)
        {
            ResourceSet set;
            if (_culturesAndResourceSets.TryGetValue(culture, out set))
            {
                return set.GetString(key);
            }
            if (!Equals(culture, CultureInfo.InvariantCulture))
            {
                return ResourceManager.GetString(key);
            }
            return string.Format(Properties.Resources.MissingTranslationFormat, key);
        }

        public void Dispose()
        {
        }
    }
}