namespace Gu.Localization.Analyzers
{
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;

    internal struct XamlFile
    {
        private XamlFile(string text, Encoding encoding)
        {
            this.Text = text;
            this.Encoding = encoding;
        }

        internal string Text { get; }

        internal Encoding Encoding { get; }

        internal static XamlFile Create(string fileName)
        {
            using (var reader = new StreamReader(File.OpenRead(fileName), Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
            {
                return new XamlFile(reader.ReadToEnd(), reader.CurrentEncoding);
            }
        }

        internal static bool TryUpdateUsage(string fileName, IPropertySymbol resource, string newName, out XamlFile result)
        {
            var xaml = XamlFile.Create(fileName);
            var pattern = $"xmlns:(?<alias>\\w+)=\"clr-namespace:{resource.ContainingType.ContainingSymbol}(\"|;)";
            if (Regex.Match(xaml.Text, pattern) is Match match &&
                match.Success &&
                match.Groups["alias"].Value is string alias &&
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
                string.Format(format, oldProperty),
                string.Format(format, newProperty));
        }
    }
}
