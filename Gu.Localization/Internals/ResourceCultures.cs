namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    internal static class ResourceCultures
    {
        private static readonly IReadOnlyList<CultureInfo> EmptyCultures = new CultureInfo[0];

        internal static IReadOnlyList<CultureInfo> GetAllCultures(string assemblyPath)
        {
            string directoryName;
            if (!TryGetDirectoryPath(assemblyPath, out directoryName))
            {
                return EmptyCultures;
            }

            var directories = Directory.EnumerateDirectories(directoryName).ToArray();
            List<CultureInfo> cultures = null;
            foreach (var directory in directories)
            {
                var cultureName = System.IO.Path.GetFileName(directory);
                if (!Culture.Exists(cultureName))
                {
                    continue;
                }

                if (Directory.EnumerateFiles(directory, "*.resources.dll", SearchOption.TopDirectoryOnly).Any())
                {
                    if (cultures == null)
                    {
                        cultures = new List<CultureInfo>();
                    }

                    cultures.Add(CultureInfo.GetCultureInfo(cultureName));
                }
            }

            return cultures ?? EmptyCultures;
        }

        internal static IReadOnlyDictionary<CultureInfo, string> CulturesAndFileNames(string assemblyPath)
        {
            string directoryName;
            if (!TryGetDirectoryPath(assemblyPath, out directoryName))
            {
                return EmptyReadOnlyDictionary<CultureInfo, string>.Default;
            }

            var resourcePattern = $"{System.IO.Path.GetFileNameWithoutExtension(assemblyPath)}.resources.dll";
            var directories = Directory.EnumerateDirectories(directoryName).ToArray();
            Dictionary<CultureInfo, string> culturesAndFiles = null;
            foreach (var directory in directories)
            {
                CultureInfo culture;
                string fileName;
                if (TryGetCultureAndFile(directory, resourcePattern, out culture, out fileName))
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

        private static bool TryGetDirectoryPath(string assemblyPath, out string directoryName)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                directoryName = null;
                return false;
            }

            if (Directory.Exists(assemblyPath))
            {
                directoryName = assemblyPath;
                return true;
            }

            var uri = new Uri(assemblyPath, UriKind.Absolute);
            if (!File.Exists(uri.LocalPath))
            {
                directoryName = null;
                return false;
            }

            directoryName = System.IO.Path.GetDirectoryName(uri.LocalPath);
            return true;
        }
    }
}
