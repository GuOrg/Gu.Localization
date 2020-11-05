namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Windows.Data;

    using Gu.Localization;

    /// <inheritdoc />
    [ValueConversion(typeof(CultureInfo), typeof(string))]
    public sealed class CultureToFlagPathConverter : IValueConverter
    {
        /// <summary>The default instance.</summary>
        public static readonly CultureToFlagPathConverter Default = new CultureToFlagPathConverter();

        private static readonly IReadOnlyDictionary<string, string> FlagNameResourceMap = CreateFlagNameResourceMap();

        /// <summary>
        /// Try get the path to the resource for the flag that matches <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">The <see cref="CultureInfo"/>.</param>
        /// <param name="path">The pack uri path to the resource.</param>
        /// <returns>True if a path was found.</returns>
        public static bool TryGetFlagPath(CultureInfo culture, [NotNullWhen(true)] out string? path)
        {
            path = null;
            if (culture is null)
            {
                return false;
            }

            if (Culture.TryGetRegion(culture, out var region) &&
                FlagNameResourceMap.TryGetValue(region.TwoLetterISORegionName, out path))
            {
                return true;
            }

            path = string.Empty;
            return false;
        }

        /// <inheritdoc />
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        public object Convert(object value, Type targetType, object parameter, CultureInfo _)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            if (value is CultureInfo culture &&
                TryGetFlagPath(culture, out var path))
            {
                return path;
            }

            return Binding.DoNothing;
        }

        /// <inheritdoc />
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(CultureToFlagPathConverter)} can only be used with {nameof(BindingMode)}.{nameof(BindingMode.OneWay)}");
        }

        private static IReadOnlyDictionary<string, string> CreateFlagNameResourceMap()
        {
            var assembly = typeof(LanguageSelector).Assembly;
            var names = assembly.GetManifestResourceNames();
            var match = names.Single(x => x.EndsWith(".g.resources", StringComparison.Ordinal));
            Debug.Assert(match != null, "match != null");
            using var stream = assembly.GetManifestResourceStream(match);
            if (stream is null)
            {
                throw new InvalidOperationException($"GetManifestResourceStream({match}) returned null");
            }

            using var reader = new ResourceReader(stream);
            var flags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var enumerator = reader.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var flagName = (string)enumerator.Key;
                Debug.Assert(flagName != null, "flag == null");
                var name = System.IO.Path.GetFileNameWithoutExtension(flagName);
                if (Culture.AllRegions.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) ||
                                                string.Equals(x.TwoLetterISORegionName, name, StringComparison.OrdinalIgnoreCase)))
                {
                    flags.Add(name, $"pack://application:,,,/Gu.Wpf.Localization;component/Flags/{name}.png");
                }
            }

            return flags;
        }
    }
}
