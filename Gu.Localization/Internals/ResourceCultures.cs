namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Text.RegularExpressions;

    /// <summary>Utility class for finding resources.</summary>
    internal static class ResourceCultures
    {
        private static readonly IReadOnlyList<CultureInfo> EmptyCultures = new CultureInfo[0];

        /// <summary>
        /// Gets all cultures found in the directory. The search is done by:
        /// 1) Enumerate all folders named with valid culture names
        /// 2) Check that the folder contains resource files
        /// The result is not cached.
        /// </summary>
        /// <param name="executingDirectory">The directory to check.</param>
        /// <returns>The cultures found. If none an empty list is returned.</returns>
        internal static IEnumerable<CultureInfo> GetAllCultures(DirectoryInfo executingDirectory)
        {
            HashSet<CultureInfo>? cultures = null;
            if (!executingDirectory.Exists)
            {
                if (Assembly.GetEntryAssembly() is { } assembly &&
                    assembly.GetType()
                        .GetMethod(
                            "InternalGetSatelliteAssembly",
                            binder: null,
                            bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
                            modifiers: null,
                            types: new[] { typeof(CultureInfo), typeof(Version), typeof(bool) }) is { } methodInfo)
                {
                    if (assembly.GetCustomAttribute<NeutralResourcesLanguageAttribute>() is { } neutralLanguageAttribute &&
                        Gu.Localization.Culture.TryGet(neutralLanguageAttribute.CultureName, out var neutralCulture))
                    {
                        if (cultures is null)
                        {
                            cultures = new HashSet<CultureInfo>(CultureInfoComparer.ByName);
                        }

                        cultures.Add(neutralCulture);
                    }

                    var method = (Func<CultureInfo, Version?, bool, Assembly?>)methodInfo.CreateDelegate(typeof(Func<CultureInfo, Version?, bool, Assembly?>), assembly);
                    foreach (var candidate in CultureInfo.GetCultures(CultureTypes.AllCultures))
                    {
                        if (method(candidate, null, false) is { })
                        {
                            if (cultures is null)
                            {
                                cultures = new HashSet<CultureInfo>(CultureInfoComparer.ByName);
                            }

                            cultures.Add(candidate);
                        }
                    }
                }

                return (IEnumerable<CultureInfo>?)cultures ?? EmptyCultures;
            }

            foreach (var directory in executingDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (Culture.TryGet(directory.Name, out var culture))
                {
                    if (directory.EnumerateFiles("*.resources.dll", SearchOption.TopDirectoryOnly).Any())
                    {
                        if (cultures is null)
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
                var neutralLanguageAttribute = entryAssembly.GetCustomAttribute<NeutralResourcesLanguageAttribute>();
                if (Culture.TryGet(neutralLanguageAttribute?.CultureName, out var neutralCulture))
                {
                    if (cultures is null)
                    {
                        cultures = new HashSet<CultureInfo>(CultureInfoComparer.ByName);
                    }

                    cultures.Add(neutralCulture);
                }

                var pattern = $"{entryAssembly.GetName().Name}\\.(?<culture>[A-Za-z]{{1,8}}(-[A-Za-z0-9]{{1,8}})*)\\.resources\\.dll";
                foreach (var name in entryAssembly.GetManifestResourceNames())
                {
                    var match = Regex.Match(name, pattern, RegexOptions.IgnoreCase);
                    if (match.Success &&
                        Culture.TryGet(match.Groups["culture"].Value, out var culture))
                    {
                        if (cultures is null)
                        {
                            cultures = new HashSet<CultureInfo>(CultureInfoComparer.ByName);
                        }

                        cultures.Add(culture);
                    }
                }
            }

            return (IEnumerable<CultureInfo>?)cultures ?? EmptyCultures;
        }

        internal static DirectoryInfo? DefaultResourceDirectory()
        {
            if (string.IsNullOrEmpty(Assembly.GetEntryAssembly()?.Location))
            {
                return null;
            }

            var assembly = typeof(ResourceCultures).Assembly;
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            var name = $"{assembly.GetName().Name}.dll";
            if (currentDirectory.Contains(name) ||
                assembly.CodeBase is null)
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
