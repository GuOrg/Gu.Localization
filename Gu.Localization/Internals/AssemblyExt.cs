namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal static class AssemblyExt
    {
        internal static IReadOnlyDictionary<CultureInfo, string> CulturesAndFileNames(this Assembly assembly)
        {
            var culturesAndFileNames = CulturesAndFileNames(assembly.Location);
            if (culturesAndFileNames != EmptyReadOnlyDictionary<CultureInfo, string>.Default)
            {
                return culturesAndFileNames;
            }

            return CulturesAndFileNames(assembly.CodeBase);
        }

        private static IReadOnlyDictionary<CultureInfo, string> CulturesAndFileNames(this string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                return EmptyReadOnlyDictionary<CultureInfo, string>.Default;
            }

            var uri = new Uri(assemblyPath, UriKind.Absolute);
            if (!File.Exists(uri.LocalPath))
            {
                return EmptyReadOnlyDictionary<CultureInfo, string>.Default;
            }

            var resourceName = $"{System.IO.Path.GetFileNameWithoutExtension(assemblyPath)}.resources.dll";
            var directoryName = System.IO.Path.GetDirectoryName(uri.LocalPath);
            var directories = Directory.EnumerateDirectories(directoryName).ToArray();
            Dictionary<CultureInfo, string> culturesAndFiles = null;
            foreach (var directory in directories)
            {
                CultureInfo culture;
                string fileName;
                if (TryGetCultureAndFile(directory, resourceName, out culture, out fileName))
                {
                    if (culturesAndFiles == null)
                    {
                        culturesAndFiles = new Dictionary<CultureInfo, string>();
                    }

                    culturesAndFiles.Add(culture, fileName);
                }
            }

            return culturesAndFiles ?? (IReadOnlyDictionary<CultureInfo, string>)EmptyReadOnlyDictionary<CultureInfo, string>.Default;
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
    }
}
