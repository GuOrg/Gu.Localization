namespace Gu.Localization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Gu.Localization;

    using NUnit.Framework;

    public partial class TranslatorTests
    {
        public class Translate
        {
            [TestCase("en", "English")]
            [TestCase("sv", "Svenska")]
            [TestCase(null, "So neutral")]
            public void TranslateResourceManagerAndNameHappyPath(string cultureName, string expected)
            {
                var culture = cultureName != null
                                  ? CultureInfo.GetCultureInfo(cultureName)
                                  : CultureInfo.InvariantCulture;
                Translator.CurrentCulture = culture;
                var actual = Translator.Translate(
                    Properties.Resources.ResourceManager,
                    nameof(Properties.Resources.AllLanguages));
                Assert.AreEqual(expected, actual);

                Translator.CurrentCulture = CultureInfo.GetCultureInfo("it");
                actual = Translator.Translate(
                    Properties.Resources.ResourceManager,
                    nameof(Properties.Resources.AllLanguages),
                    culture);
                Assert.AreEqual(expected, actual);
            }

            [TestCase(nameof(Properties.Resources.NeutralOnly), null, "So neutral")]
            [TestCase(nameof(Properties.Resources.AllLanguages), "en", "English")]
            [TestCase(nameof(Properties.Resources.AllLanguages), "sv", "Svenska")]
            [TestCase(nameof(Properties.Resources.AllLanguages), null, "So neutral")]
            public void HappyPath(string key, string culture, string expected)
            {
                Translator.CurrentCulture = culture == null
                                                ? CultureInfo.InvariantCulture
                                                : CultureInfo.GetCultureInfo(culture);
                var actual = Translator.Translate(Properties.Resources.ResourceManager, key);
                Assert.AreEqual(expected, actual);

                foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                                                  .OfType<ErrorHandling>())
                {
                    actual = Translator.Translate(Properties.Resources.ResourceManager, key, errorHandling);
                    Assert.AreEqual(expected, actual);
                }

                foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                                                  .OfType<ErrorHandling>())
                {
                    Translator.ErrorHandling = errorHandling;
                    actual = Translator.Translate(Properties.Resources.ResourceManager, key);
                    Assert.AreEqual(expected, actual);
                }
            }

            [TestCase(null, "sv", "key == null")]
            [TestCase("Missing", "sv", "!Missing!")]
            [TestCase(nameof(Properties.Resources.EnglishOnly), "sv", "_EnglishOnly_")]
            [TestCase(nameof(Properties.Resources.EnglishOnly), "it", "~EnglishOnly~")]
            [TestCase(nameof(Properties.Resources.AllLanguages), "it", "~So neutral~")]
            public void ErrorMessages(string key, string culture, string expected)
            {
                Translator.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                Translator.ErrorHandling = ErrorHandling.Throw;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, key, ErrorHandling.ReturnErrorInfo);
                Assert.AreEqual(expected, actual);

                Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
                actual = Translator.Translate(Properties.Resources.ResourceManager, key);
                Assert.AreEqual(expected, actual);
            }

            [TestCase("Missing", null, "The resourcemanager Gu.Localization.Tests.Properties.Resources does not have the key: Missing\r\nParameter name: key")]
            [TestCase("Missing", "sv", "The resourcemanager Gu.Localization.Tests.Properties.Resources does not have the key: Missing\r\nParameter name: key")]
            [TestCase(nameof(Properties.Resources.EnglishOnly), "sv", "The resourcemanager Gu.Localization.Tests.Properties.Resources does not have a translation for the key: EnglishOnly for the culture: sv\r\nParameter name: key")]
            [TestCase(nameof(Properties.Resources.AllLanguages), "it", "The resourcemanager Gu.Localization.Tests.Properties.Resources does not have a translation for the culture: it\r\nParameter name: culture")]
            [TestCase(nameof(Properties.Resources.NeutralOnly), "it", "The resourcemanager Gu.Localization.Tests.Properties.Resources does not have a translation for the culture: it\r\nParameter name: culture")]
            [TestCase("MissingKey", "it", "The resourcemanager Gu.Localization.Tests.Properties.Resources does not have a translation for the culture: it\r\nParameter name: culture")]
            public void Throws(string key, string culture, string expected)
            {
                Translator.CurrentCulture = culture == null
                                                ? CultureInfo.InvariantCulture
                                                : CultureInfo.GetCultureInfo(culture);
                Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;

                var exception =
                    Assert.Throws<ArgumentOutOfRangeException>(
                        () => Translator.Translate(Properties.Resources.ResourceManager, key, ErrorHandling.Throw));
                Assert.AreEqual(expected, exception.Message);

                Translator.ErrorHandling = ErrorHandling.Throw;
                exception =
                    Assert.Throws<ArgumentOutOfRangeException>(
                        () => Translator.Translate(Properties.Resources.ResourceManager, key));
                Assert.AreEqual(expected, exception.Message);

                exception =
                    Assert.Throws<ArgumentOutOfRangeException>(
                        () => Translator.Translate(Properties.Resources.ResourceManager, key, ErrorHandling.Default));
                Assert.AreEqual(expected, exception.Message);

                exception =
                    Assert.Throws<ArgumentOutOfRangeException>(
                        () => Translator.Translate(Properties.Resources.ResourceManager, key, ErrorHandling.Throw));
                Assert.AreEqual(expected, exception.Message);
            }

        }
    }
}