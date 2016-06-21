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

            internal static IReadOnlyDictionary<string, string> ReadResourceSet(string filename)
            {
                try
                {
                    var cultureAssembly = Assembly.Load(File.ReadAllBytes(filename));
                    return ReadResourceSet(cultureAssembly);
                }
                catch (Exception)
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

            private static IReadOnlyDictionary<CultureInfo, string> FindCultureFileNames(Uri assemblyLocation)
            {
                Dictionary<CultureInfo, string> cultures = null;
                var resourceFileName = $"{Path.GetFileNameWithoutExtension(assemblyLocation.AbsolutePath)}.resources.dll";
                var directoryName = Path.GetDirectoryName(assemblyLocation.AbsolutePath);
                foreach (var folder in Directory.EnumerateDirectories(directoryName))
                {
                    var folderName = Path.GetFileName(folder);
                    if (Culture.Exists(folderName))
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

                        cultures.Add(cultureInfo, "");
                    }
                }

                return cultures ?? (IReadOnlyDictionary<CultureInfo, string>)EmptyReadOnlyDictionary<CultureInfo, string>.Default;
            }

            internal static IReadOnlyDictionary<CultureInfo, string> CulturesAndFileNames(DirectoryInfo executingDirectory)
            {
                throw new NotImplementedException("message");
                //if (!executingDirectory.Exists)
                //{
                //    return EmptyReadOnlyDictionary<CultureInfo, string>.Default;
                //}

                //var resourcePattern = $"{System.IO.Path.GetFileNameWithoutExtension(assemblyPath)}.resources.dll";
                //var directories = Directory.EnumerateDirectories(directoryName).ToArray();
                //Dictionary<CultureInfo, string> culturesAndFiles = null;
                //foreach (var directory in directories)
                //{
                //    CultureInfo culture;
                //    string fileName;
                //    if (TryGetCultureAndFile(directory, resourcePattern, out culture, out fileName))
                //    {
                //        if (culturesAndFiles == null)
                //        {
                //            culturesAndFiles = new Dictionary<CultureInfo, string>();
                //        }

                //        culturesAndFiles.Add(culture, fileName);
                //    }
                //}

                //return culturesAndFiles ?? (IReadOnlyDictionary<CultureInfo, string>)EmptyReadOnlyDictionary<CultureInfo, string>.Default;
            }

            private static bool TryGetCultureAndFile(string directory, string searchPattern, out CultureInfo culture, out string fileName)
            {
                var folderName = System.IO.Path.GetFileName(directory);
                if (!Culture.Exists(folderName))
                {
                    culture = null;
                    fileName = null;
                    return false;
                }

                fileName = Directory.EnumerateFiles(directory, searchPattern, SearchOption.TopDirectoryOnly).SingleOrDefault();
                if (fileName == null)
                {
                    culture = null;
                    return false;
                }

                culture = CultureInfo.GetCultureInfo(folderName);
                return true;
            }

            private static string FindResourceName(Assembly assembly)
            {
                // Gu.Localization.Tests.Properties.Resources.resources
                var names = assembly.GetManifestResourceNames();
                var resourceName = names.SingleOrDefault(x => x.StartsWith(assembly.GetName().Name) && x.EndsWith(".Resources.resources"));
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
