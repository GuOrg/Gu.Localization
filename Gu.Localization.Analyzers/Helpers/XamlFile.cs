namespace Gu.Localization.Analyzers
{
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;

    internal record struct XamlFile(string Text, Encoding Encoding)
    {
        internal static XamlFile Create(string fileName)
        {
            using var reader = new StreamReader(File.OpenRead(fileName), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            return new XamlFile(reader.ReadToEnd(), reader.CurrentEncoding);
        }

        internal static bool TryUpdateUsage(string fileName, IPropertySymbol resource, string newName, out XamlFile result)
        {
            var xaml = XamlFile.Create(fileName);
            var pattern = $"xmlns:(?<alias>\\w+)=\"clr-namespace:{resource.ContainingType.ContainingSymbol}(\"|;)";
            if (Regex.Match(xaml.Text, pattern) is { Success: true } match &&
                match.Groups["alias"].Value is { } alias &&
                !string.IsNullOrEmpty(alias))
            {
                var oldProperty = $"{alias}:{resource.ContainingType.Name}.{resource.Name}";
                var newProperty = $"{alias}:{resource.ContainingType.Name}.{newName}";
                var updated = Replace(xaml.Text, " {0}}}", oldProperty, newProperty);
                updated = Replace(updated, "({0})", oldProperty, newProperty);
                updated = Replace(updated, "\"{0}\"", oldProperty, newProperty);
                if (updated != xaml.Text)
                {
                    result = new XamlFile(updated, xaml.Encoding);
                    return true;
                }
            }

            result = default;
            return false;
        }

        private static string Replace(string xaml, string format, string oldProperty, string newProperty)
        {
            return xaml.Replace(
                string.Format(CultureInfo.InvariantCulture, format, oldProperty),
                string.Format(CultureInfo.InvariantCulture, format, newProperty));
        }
    }
}
