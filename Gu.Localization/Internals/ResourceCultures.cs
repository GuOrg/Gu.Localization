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
        /// <summary>
        /// Gets all cultures found in the directory. The search is done by:
        /// 1) Enumerate all folders named with valid culture names
        /// 2) Check that the folder contains resource files
        /// The result is not cached.
        /// </summary>
        /// <param name="executingDirectory">The directory to check.</param>
        /// <returns>The cultures found. If none an empty list is returned.</returns>
        internal static IEnumerable<CultureInfo> GetAllCultures(DirectoryInfo? executingDirectory)
        {
            if (executingDirectory is null)
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
                        yield return neutralCulture;
                    }

                    var method = (Func<CultureInfo, Version?, bool, Assembly?>)methodInfo.CreateDelegate(typeof(Func<CultureInfo, Version?, bool, Assembly?>), assembly);
                    foreach (var candidate in CultureInfo.GetCultures(CultureTypes.AllCultures))
                    {
                        if (method(candidate, null, false) is { })
                        {
                            yield return candidate;
                        }
                    }
                }

                yield break;
            }

            foreach (var directory in executingDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (Culture.TryGet(directory.Name, out var culture))
                {
                    if (directory.EnumerateFiles("*.resources.dll", SearchOption.TopDirectoryOnly).Any())
                    {
                        yield return culture;
                    }
                }
            }

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                var neutralLanguageAttribute = entryAssembly.GetCustomAttribute<NeutralResourcesLanguageAttribute>();
                if (Culture.TryGet(neutralLanguageAttribute?.CultureName, out var neutralCulture))
                {
                    yield return neutralCulture;
                }

                var pattern = $"{entryAssembly.GetName().Name}\\.(?<culture>[A-Za-z]{{1,8}}(-[A-Za-z0-9]{{1,8}})*)\\.resources\\.dll";
                foreach (var name in entryAssembly.GetManifestResourceNames())
                {
                    var match = Regex.Match(name, pattern, RegexOptions.IgnoreCase);
                    if (match.Success &&
                        Culture.TryGet(match.Groups["culture"].Value, out var culture))
                    {
                        yield return culture;
                    }
                }
            }
        }

        internal static DirectoryInfo? DefaultResourceDirectory()
        {
            if (Assembly.GetEntryAssembly() is { } entryAssembly)
            {
                if (string.IsNullOrEmpty(entryAssembly.Location))
                {
                    // net 5 single exe.
                    return null;
                }

                return new DirectoryInfo(Path.GetDirectoryName(entryAssembly.Location)!);
            }

            var assembly = typeof(ResourceCultures).Assembly;
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            if (currentDirectory.Contains($"{assembly.GetName().Name}.dll", SearchOption.AllDirectories))
            {
                return currentDirectory;
            }

            return null;
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
