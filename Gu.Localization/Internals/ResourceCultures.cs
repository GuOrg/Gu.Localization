namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    /// <summary>Utility class for finding resources.</summary>
    internal static class ResourceCultures
    {
        private static readonly IReadOnlyList<CultureInfo> EmptyCultures = new CultureInfo[0];

        /// <summary>
        /// Gets all cultures found in the directory. The search is donde by:
        /// 1) Enumerate all folders named with valid culture names
        /// 2) Check that the folder contains resource files
        /// The result is not cached
        /// </summary>
        /// <param name="executingDirectory">The directory to check</param>
        /// <returns>The cultures found. If none an empty list is returned.</returns>
        internal static IReadOnlyList<CultureInfo> GetAllCultures(DirectoryInfo executingDirectory)
        {
            if (!executingDirectory.Exists)
            {
                return EmptyCultures;
            }

            HashSet<CultureInfo> cultures = null;
            foreach (var directory in executingDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (Culture.TryGet(directory.Name, out var culture))
                {
                    if (directory.EnumerateFiles("*.resources.dll", SearchOption.TopDirectoryOnly).Any())
                    {
                        if (cultures == null)
                        {
                            cultures = new HashSet<CultureInfo>(CultureInfoComparer.ByName);
                        }

                        cultures.Add(culture);
                    }
                }
            }

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                var neutralLanguageAttribute = (NeutralResourcesLanguageAttribute)Attribute.GetCustomAttribute(entryAssembly, typeof(NeutralResourcesLanguageAttribute));
                var name = neutralLanguageAttribute?.CultureName;
                if (name != null && Culture.TryGet(name, out var neutralCulture))
                {
                    if (cultures == null)
                    {
                        cultures = new HashSet<CultureInfo>(CultureInfoComparer.ByName);
                    }

                    cultures.Add(neutralCulture);
                }
            }

            return cultures?.OrderBy(x => x.Name).ToArray() ?? EmptyCultures;
        }

        internal static DirectoryInfo DefaultResourceDirectory()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            var assembly = typeof(ResourceCultures).Assembly;
            var name = $"{assembly.GetName().Name}.dll";
            if (currentDirectory.Contains(name))
            {
                return currentDirectory;
            }

            return new DirectoryInfo(System.IO.Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath));
        }

        private static bool Contains(
            this DirectoryInfo directory,
            string fileName,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return directory.EnumerateFiles(fileName, searchOption)
                            .Any();
        }
    }
}
