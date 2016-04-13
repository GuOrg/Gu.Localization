namespace Gu.Localization
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    internal static class ResourceCultures
    {
        private static readonly IReadOnlyList<CultureInfo> EmptyCultures = new CultureInfo[0];

        internal static IReadOnlyList<CultureInfo> GetAllCultures(DirectoryInfo executingDirectory)
        {
            if (!executingDirectory.Exists)
            {
                return EmptyCultures;
            }

            List<CultureInfo> cultures = null;
            foreach (var directory in executingDirectory.EnumerateDirectories())
            {
                var cultureName = directory.Name;
                if (!Culture.Exists(directory.Name))
                {
                    continue;
                }

                if (directory.EnumerateFiles("*.resources.dll", SearchOption.TopDirectoryOnly).Any())
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
    }
}
