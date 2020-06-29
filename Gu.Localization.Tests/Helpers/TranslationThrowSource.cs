namespace Gu.Localization.Tests
{
    using System.Collections.Generic;
    using System.Globalization;

    public class TranslationThrowSource : List<TranslationThrowSource.ErrorData>
    {
        private static readonly CultureInfo Sv = CultureInfo.GetCultureInfo("sv");
        private static readonly CultureInfo It = CultureInfo.GetCultureInfo("it");

        public TranslationThrowSource()
        {
            this.Add(null, Sv, ErrorHandling.Throw, "key is null\r\nParameter name: key");
            this.Add(null, null, ErrorHandling.Throw, "key is null\r\nParameter name: key");
            this.Add("Missing", Sv, ErrorHandling.Throw, "The ResourceManager Gu.Localization.Tests.Properties.Resources does not have the key: Missing\r\n" +
                                                         "Fix the problem by adding a translation for the key 'Missing'\r\n" +
                                                         "Parameter name: key");
            this.Add("Missing", null, ErrorHandling.Throw, "The ResourceManager Gu.Localization.Tests.Properties.Resources does not have the key: Missing\r\n" +
                                                           "Fix the problem by adding a translation for the key 'Missing'\r\n" +
                                                           "Parameter name: key");
            this.Add(nameof(Properties.Resources.EnglishOnly), Sv, ErrorHandling.Throw, "The ResourceManager Gu.Localization.Tests.Properties.Resources does not have a translation for the key: EnglishOnly for the culture: sv\r\n" +
                                                                                        "Fix by either of:\r\n" +
                                                                                        "  - Add a translation for the key 'EnglishOnly' for the culture 'sv'\r\n" +
                                                                                        "  - If falling back to neutral is desired specify ErrorHandling.ReturnErrorInfoPreserveNeutral\r\n" +
                                                                                        "Parameter name: key");
            this.Add(nameof(Properties.Resources.AllLanguages), It, ErrorHandling.Throw, "The ResourceManager Gu.Localization.Tests.Properties.Resources does not have a translation for the culture: it\r\n" +
                                                                                         "Fix by either of:\r\n" +
                                                                                         "  - Add a resource file for the culture it\r\n" +
                                                                                         "  - If falling back to neutral is desired specify ErrorHandling.ReturnErrorInfoPreserveNeutral\r\n" +
                                                                                         "Parameter name: language");
            this.Add(nameof(Properties.Resources.NeutralOnly), It, ErrorHandling.Throw, "The ResourceManager Gu.Localization.Tests.Properties.Resources does not have a translation for the culture: it\r\n" +
                                                                                        "Fix by either of:\r\n" +
                                                                                        "  - Add a resource file for the culture it\r\n" +
                                                                                        "  - If falling back to neutral is desired specify ErrorHandling.ReturnErrorInfoPreserveNeutral\r\n" +
                                                                                        "Parameter name: language");
        }

        private void Add(string? key, CultureInfo? culture, ErrorHandling errorHandling, string expected)
        {
            this.Add(new ErrorData(key, culture, errorHandling, expected));
        }

        public class ErrorData
        {
            public ErrorData(string? key, CultureInfo? culture, ErrorHandling errorHandling, string expectedMessage)
            {
                this.Key = key;
                this.Culture = culture;
                this.ErrorHandling = errorHandling;
                this.ExpectedMessage = expectedMessage;
            }

            public string? Key { get; }

            public CultureInfo? Culture { get; }

            public ErrorHandling ErrorHandling { get; }

            public string ExpectedMessage { get; }

            public override string ToString()
            {
                return $"Key: {this.Key}, Culture: {this.Culture}, ErrorHandling: {this.ErrorHandling}, ExpectedTranslation: {this.ExpectedMessage}";
            }
        }
    }
}
