// ReSharper disable AssignNullToNotNullAttribute
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

    public partial class Translations
    {
        private static class FromFiles
        {
            internal static Translations FindTranslations(Assembly assembly)
            {
                var neutralResourceSet = ReadResourceSet(assembly);
                if (neutralResourceSet == null || neutralResourceSet.Count == 0)
                {
                    return null;
                }

                var resourceName = FindResourceName(assembly);
                var baseName = GetBaseName(resourceName);
                var cultures = FindCultureFileNames(assembly);
                return new Translations(baseName, neutralResourceSet, cultures);
            }

            internal static IReadOnlyDictionary<CultureInfo, string> FindCultureFileNames(Assembly assembly)
            {
                var cultures = FindCultureFileNames(new Uri(assembly.Location));
                if (cultures != EmptyReadOnlyDictionary<CultureInfo, string>.Default)
                {
                    return cultures;
                }

                return FindCultureFileNames(new Uri(assembly.EscapedCodeBase, UriKind.Absolute));
            }

            // ReSharper disable once UnusedMember.Local
            internal static IReadOnlyDictionary<string, string> ReadResourceSet(string filename)
            {
                try
                {
                    var cultureAssembly = Assembly.Load(File.ReadAllBytes(filename));
                    return ReadResourceSet(cultureAssembly);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    return null;
                }
            }

            internal static IReadOnlyDictionary<string, string> ReadResourceSet(Assembly assembly)
            {
                var resourceName = FindResourceName(assembly);
                if (resourceName == null)
                {
                    return null;
                }

                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new ResourceReader(stream);
                var dictionary = reader.OfType<DictionaryEntry>()
                                       .Where(x => x.Key is string && x.Value is string)
                                       .ToDictionary(x => (string)x.Key, x => (string)x.Value);
                return dictionary;
            }

            private static IReadOnlyDictionary<CultureInfo, string> FindCultureFileNames(Uri assemblyLocation)
            {
                Dictionary<CultureInfo, string> cultures = null;
                var resourceFileName = $"{Path.GetFileNameWithoutExtension(assemblyLocation.AbsolutePath)}.resources.dll";
                var directoryName = Path.GetDirectoryName(assemblyLocation.AbsolutePath);
                foreach (var folder in Directory.EnumerateDirectories(directoryName))
                {
                    var folderName = Path.GetFileName(folder);
                    if (Culture.TryGet(folderName, out _))
                    {
                        var match = Directory.EnumerateFiles(folder, resourceFileName, SearchOption.TopDirectoryOnly)
                                             .SingleOrDefault();
                        if (match == null)
                        {
                            continue;
                        }

                        var cultureInfo = CultureInfo.GetCultureInfo(folderName);
                        if (cultures == null)
                        {
                            cultures = new Dictionary<CultureInfo, string>(CultureInfoComparer.ByName);
                        }

                        cultures.Add(cultureInfo, string.Empty);
                    }
                }

                return cultures ?? (IReadOnlyDictionary<CultureInfo, string>)EmptyReadOnlyDictionary<CultureInfo, string>.Default;
            }

            private static string FindResourceName(Assembly assembly)
            {
                // Gu.Localization.Tests.Properties.Resources.resources
                var names = assembly.GetManifestResourceNames();
                var resourceName = names.SingleOrDefault(x => x.StartsWith(assembly.GetName().Name) && x.EndsWith(".Resources.resources", StringComparison.Ordinal));
                return resourceName;
            }

            private static string GetBaseName(string resourceName)
            {
                var end = resourceName.LastIndexOf(".resources", StringComparison.OrdinalIgnoreCase);
                if (end < 0)
                {
                    throw new InvalidOperationException($"Could not get BaseName from resourceName: {resourceName}");
                }

                return resourceName.Substring(0, resourceName.LastIndexOf(".resources", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
