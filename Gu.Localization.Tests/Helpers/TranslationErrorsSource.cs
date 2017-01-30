namespace Gu.Localization.Tests
{
    using System.Collections.Generic;
    using System.Globalization;

    public class TranslationErrorsSource : List<TranslationErrorsSource.ErrorData>
    {
        private static readonly CultureInfo Sv = CultureInfo.GetCultureInfo("sv");
        private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en");
        private static readonly CultureInfo It = CultureInfo.GetCultureInfo("it");

        public TranslationErrorsSource()
        {
            this.Add(null, Sv, ErrorHandling.ReturnErrorInfo, "key == null");
            this.Add(null, Sv, ErrorHandling.ReturnErrorInfoPreserveNeutral, "key == null");
            this.Add("Missing", Sv, ErrorHandling.ReturnErrorInfo, "!Missing!");
            this.Add("Missing", Sv, ErrorHandling.ReturnErrorInfoPreserveNeutral, "!Missing!");
            this.Add(nameof(Properties.Resources.EnglishOnly), En, ErrorHandling.ReturnErrorInfo, "English");
            this.Add(nameof(Properties.Resources.EnglishOnly), En, ErrorHandling.ReturnErrorInfoPreserveNeutral, "English");
            this.Add(nameof(Properties.Resources.EnglishOnly), Sv, ErrorHandling.ReturnErrorInfoPreserveNeutral, string.Empty);
            this.Add(nameof(Properties.Resources.EnglishOnly), Sv, ErrorHandling.ReturnErrorInfo, "_EnglishOnly_");
            this.Add(nameof(Properties.Resources.EnglishOnly), It, ErrorHandling.ReturnErrorInfoPreserveNeutral, string.Empty);
            this.Add(nameof(Properties.Resources.EnglishOnly), It, ErrorHandling.ReturnErrorInfo, "~EnglishOnly~");
            this.Add(nameof(Properties.Resources.AllLanguages), En, ErrorHandling.ReturnErrorInfoPreserveNeutral, "English");
            this.Add(nameof(Properties.Resources.AllLanguages), En, ErrorHandling.ReturnErrorInfo, "English");
            this.Add(nameof(Properties.Resources.AllLanguages), It, ErrorHandling.ReturnErrorInfoPreserveNeutral, "So neutral");
            this.Add(nameof(Properties.Resources.AllLanguages), It, ErrorHandling.ReturnErrorInfo, "~So neutral~");
            this.Add(nameof(Properties.Resources.NeutralOnly), Sv, ErrorHandling.ReturnErrorInfoPreserveNeutral, "So neutral");
            this.Add(nameof(Properties.Resources.NeutralOnly), Sv, ErrorHandling.ReturnErrorInfo, "_So neutral_");
        }

        private void Add(string key, CultureInfo culture, ErrorHandling errorHandling, string expected)
        {
            this.Add(new ErrorData(key, culture, errorHandling, expected));
        }

        public class ErrorData
        {
            public ErrorData(string key, CultureInfo culture, ErrorHandling errorHandling, string expectedTranslation)
            {
                this.Key = key;
                this.Culture = culture;
                this.ErrorHandling = errorHandling;
                this.ExpectedTranslation = expectedTranslation;
            }

            public string Key { get; }

            public CultureInfo Culture { get; }

            public ErrorHandling ErrorHandling { get; }

            public string ExpectedTranslation { get; }

            public override string ToString()
            {
                return $"Key: {this.Key}, Culture: {this.Culture}, ErrorHandling: {this.ErrorHandling}, ExpectedTranslation: {this.ExpectedTranslation}";
            }
        }
    }
}