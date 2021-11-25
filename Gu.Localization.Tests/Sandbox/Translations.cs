﻿namespace Gu.Localization.Tests.Sandbox
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public partial class Translations
    {
        private static readonly ConcurrentDictionary<Assembly, Translations?> Cache = new();

        //// ReSharper disable NotAccessedField.Local for debugging
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IReadOnlyDictionary<string, string> neutralMap;
        private readonly IReadOnlyDictionary<CultureInfo, string> cultureFileMap;
#pragma warning restore IDE0052 // Remove unread private members
        //// ReSharper restore NotAccessedField.Local

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

        public static Translations? GetOrCreate(Assembly assembly)
        {
            return Cache.GetOrAdd(assembly, x => Translations.FromFiles.FindTranslations(x));
        }
    }
}
