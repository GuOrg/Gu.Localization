namespace Gu.Localization.Analyzers
{
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;

    internal static class XamlFile
    {
        internal static void UpdateUsage(string fileName, IPropertySymbol resource, string newName)
        {
            var xaml = TextWithEncoding.Create(fileName);
            var pattern = $"xmlns:(?<alias>\\w+)=\"clr-namespace:{resource.ContainingType.ContainingSymbol}(\"|;)";
            if (Regex.Match(xaml.Text, pattern) is Match match &&
                match.Success &&
                match.Groups["alias"].Value is string alias &&
                !string.IsNullOrEmpty(alias))
            {
                var updated = xaml.Text.Replace(
                    $"{alias}:{resource.ContainingType.Name}.{resource.Name}",
                    $"{alias}:{resource.ContainingType.Name}.{newName}");
                if (updated != xaml.Text)
                {
                    File.WriteAllText(fileName, updated, xaml.Encoding);
                }
            }
        }

        private struct TextWithEncoding
        {
            public TextWithEncoding(string text, Encoding encoding)
            {
                this.Text = text;
                this.Encoding = encoding;
            }

            public string Text { get; }

            public Encoding Encoding { get; }

            public static TextWithEncoding Create(string fileName)
            {
                using (var reader = new StreamReader(File.OpenRead(fileName), Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                {
                    return new TextWithEncoding(reader.ReadToEnd(), reader.CurrentEncoding);
                }
            }
        }
    }
}
