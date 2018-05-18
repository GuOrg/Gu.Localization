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

        public string Text { get; }

        public Encoding Encoding { get; }

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
                var updated = xaml.Text.Replace(
                    $"{alias}:{resource.ContainingType.Name}.{resource.Name}",
                    $"{alias}:{resource.ContainingType.Name}.{newName}");
                if (updated != xaml.Text)
                {
                    result = new XamlFile(updated, xaml.Encoding);
                    return true;
                }
            }

            result = default(XamlFile);
            return false;
        }
    }
}
