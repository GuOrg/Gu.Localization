namespace Gu.Localization.Tests
{
    using System.Collections.Generic;
    using System.Globalization;

    public class TranslationSource : List<TranslationSource.Row>
    {
        private static readonly CultureInfo Sv = CultureInfo.GetCultureInfo("sv");
        private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en");

        public TranslationSource()
        {
            this.Add(nameof(Properties.Resources.AllLanguages), CultureInfo.InvariantCulture, "So neutral");
            this.Add(nameof(Properties.Resources.AllLanguages), Sv, "Svenska");
            this.Add(nameof(Properties.Resources.AllLanguages), Sv, "Svenska");
            this.Add(nameof(Properties.Resources.EnglishOnly), En, "English");
            this.Add(nameof(Properties.Resources.NeutralOnly), CultureInfo.InvariantCulture, "So neutral");
        }

        private void Add(string key, CultureInfo culture, string expected)
        {
            this.Add(new Row(key, culture, expected));
        }

        public class Row
        {
            public Row(string key, CultureInfo culture, string expectedTranslation)
            {
                this.Key = key;
                this.Culture = culture;
                this.ExpectedTranslation = expectedTranslation;
            }

            public string Key { get; }

            public CultureInfo Culture { get; }

            public string ExpectedTranslation { get; }

            public override string ToString()
            {
                return $"Key: {this.Key}, Culture: {this.Culture?.Name ?? "null"}, ExpectedTranslation: {this.ExpectedTranslation}";
            }
        }
    }
}
