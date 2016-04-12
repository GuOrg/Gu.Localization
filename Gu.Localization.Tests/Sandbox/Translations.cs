namespace Gu.Localization.Tests.Sandbox
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    public partial class Translations
    {
        private static readonly ConcurrentDictionary<Assembly, Translations> Cache = new ConcurrentDictionary<Assembly, Translations>();

        private readonly IReadOnlyDictionary<string, string> neutralMap;
        private readonly IReadOnlyDictionary<CultureInfo, string> cultureFileMap;

        private Translations(
            string baseName,
            IReadOnlyDictionary<string, string> neutralMap,
            IReadOnlyDictionary<CultureInfo, string> cultureFileMap)
        {
            this.neutralMap = neutralMap;
            this.cultureFileMap = cultureFileMap;
            this.BaseName = baseName;
            this.Cultures = cultureFileMap.Keys.ToArray();
        }

        public string BaseName { get; }

        public IReadOnlyList<CultureInfo> Cultures { get; }

        public static Translations GetOrCreate(Assembly assembly)
        {
            return Cache.GetOrAdd(assembly, FromFiles.FindTranslations);
        }
    }
}
