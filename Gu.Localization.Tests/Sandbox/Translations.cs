namespace Gu.Localization.Tests.Sandbox
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using NUnit.Framework;

    public class Translations
    {
        private static readonly IReadOnlyDictionary<CultureInfo, string> EmptyCultureFileMap = new Dictionary<CultureInfo, string>();
        private readonly IReadOnlyDictionary<string, string> neutralMap;
        private readonly IReadOnlyDictionary<CultureInfo, string> cultureFileMap;

        private Translations(
            string baseName,
            IReadOnlyDictionary<string, string> neutralMap,
            IReadOnlyDictionary<CultureInfo, string> cultureFileMap)
        {
            this.neutralMap = neutralMap;
            this.cultureFileMap = cultureFileMap;
            BaseName = baseName;
            Cultures = cultureFileMap.Keys.ToArray();
        }

        public string BaseName { get; }

        public IReadOnlyList<CultureInfo> Cultures { get; }

        public static Translations GetOrCreate(Assembly assembly)
        {
            var neutralResourceSet = ReadResourceSet(assembly);
            if (neutralResourceSet == null || neutralResourceSet.Count == 0)
            {
                return null;
            }

            var resourceName = assembly.GetManifestResourceNames()
                                       .SingleOrDefault(x => !x.EndsWith(".g.resources"));
            var baseName = resourceName.Substring(0, resourceName.LastIndexOf(".resources", StringComparison.OrdinalIgnoreCase));
            var cultures = FindCultures(assembly);
            return new Translations(baseName, neutralResourceSet, cultures);
        }

        private static IReadOnlyDictionary<CultureInfo, string> FindCultures(Assembly assembly)
        {
            var cultures = FindCultures(new Uri(assembly.Location));
            if (cultures != EmptyReadOnlyDictionary<CultureInfo, string>.Default)
            {
                return cultures;
            }

            return FindCultures(new Uri(assembly.EscapedCodeBase, UriKind.Absolute));
        }

        private static IReadOnlyDictionary<CultureInfo, string> FindCultures(Uri assemblyLocation)
        {
            Dictionary<CultureInfo, string> cultures = null;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(assemblyLocation.AbsolutePath) + "resources.dll";
            foreach (var folder in Directory.EnumerateDirectories(System.IO.Path.GetDirectoryName(assemblyLocation.AbsolutePath)))
            {
                var folderName = System.IO.Path.GetFileName(folder);
                if (Culture.Exists(folderName))
                {
                    if (Directory.EnumerateFiles(folder).SingleOrDefault(x=>x.))
                        var cultureInfo = CultureInfo.GetCultureInfo(folderName);
                    if (cultures == null)
                    {
                        cultures = new Dictionary<CultureInfo, string>(CultureInfoComparer.Default);
                    }

                    cultures.Add(cultureInfo, "");
                }
            }

            return cultures ?? (IReadOnlyDictionary<CultureInfo, string>)EmptyReadOnlyDictionary<CultureInfo, string>.Default;
        }

        private static IReadOnlyDictionary<string, string> ReadResourceSet(Assembly assembly)
        {
            var names = assembly.GetManifestResourceNames();
            var resourceName = names.SingleOrDefault(x => !x.EndsWith(".g.resources"));
            if (resourceName == null)
            {
                return null;
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new ResourceReader(stream))
                {
                    var dictionary = reader.OfType<DictionaryEntry>()
                                           .Where(x => x.Key is string && x.Value is string)
                                           .ToDictionary(x => (string)x.Key, x => (string)x.Value);
                    return dictionary;
                }
            }
        }

    }

    public class TranslationsTests
    {
        [Test]
        public void GetOrCreate()
        {
            var resourceManager = Properties.Resources.ResourceManager;
            var translations = Translations.GetOrCreate(GetType().Assembly);
        }
    }
}
