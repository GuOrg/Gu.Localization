namespace Gu.Localization.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;

    using NUnit.Framework;

    public partial class TranslatorGenericTests
    {
        [TestCase("AllLanguages", "en", "English")]
        [TestCase("AllLanguages", "sv", "Svenska")]
        [TestCase("AllLanguages", null, "So neutral")]
        public void HappyPath(string key, string culture, string expected)
        {
            Translator.CurrentCulture = culture == null
                                            ? CultureInfo.InvariantCulture
                                            : CultureInfo.GetCultureInfo(culture);
            var actual = Translator<Properties.Resources>.Translate(nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual(expected, actual);

            foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling)).OfType<ErrorHandling>())
            {
                actual = Translator<Properties.Resources>.Translate(nameof(Properties.Resources.AllLanguages), errorHandling);
                Assert.AreEqual(expected, actual);
            }

            foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling)).OfType<ErrorHandling>())
            {
                Translator.ErrorHandling = errorHandling;
                actual = Translator<Properties.Resources>.Translate(nameof(Properties.Resources.AllLanguages));
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CreateTranslation()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var translation = Translator<Properties.Resources>.GetOrCreateTranslation(nameof(Properties.Resources.AllLanguages));

            Assert.AreEqual("English", translation.Translated);
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual("Svenska", translation.Translated);
        }

        [TestCase(null, "sv", "key == null")]
        [TestCase("Missing", "sv", "!Missing!")]
        [TestCase(nameof(Properties.Resources.EnglishOnly), "sv", "_EnglishOnly_")]
        [TestCase(nameof(Properties.Resources.EnglishOnly), "it", "~EnglishOnly~")]
        [TestCase(nameof(Properties.Resources.AllLanguages), "it", "~So neutral~")]
        public void ErrorMessages(string key, string culture, string expected)
        {
            Translator.CurrentCulture = culture == null
                                            ? CultureInfo.InvariantCulture
                                            : CultureInfo.GetCultureInfo(culture);
            Translator.ErrorHandling = ErrorHandling.Throw;
            var actual = Translator<Properties.Resources>.Translate(key, ErrorHandling.ReturnErrorInfo);
            Assert.AreEqual(expected, actual);

            Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
            actual = Translator<Properties.Resources>.Translate(key);
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

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(key, ErrorHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Translator.ErrorHandling = ErrorHandling.Throw;
            exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(key));
            Assert.AreEqual(expected, exception.Message);

            exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(key, ErrorHandling.Default));
            Assert.AreEqual(expected, exception.Message);

            exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(key, ErrorHandling.Throw));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ThrowsWhenKeyIsNull()
        {
            var key = (string)null;
            var expected = "Value cannot be null.\r\nParameter name: key";
            Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;

            var exception = Assert.Throws<ArgumentNullException>(() => Translator<Properties.Resources>.Translate(key, ErrorHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Translator.ErrorHandling = ErrorHandling.Throw;
            exception = Assert.Throws<ArgumentNullException>(() => Translator<Properties.Resources>.Translate(key));
            Assert.AreEqual(expected, exception.Message);

            exception = Assert.Throws<ArgumentNullException>(() => Translator<Properties.Resources>.Translate(key, ErrorHandling.Default));
            Assert.AreEqual(expected, exception.Message);

            exception = Assert.Throws<ArgumentNullException>(() => Translator<Properties.Resources>.Translate(key, ErrorHandling.Throw));
            Assert.AreEqual(expected, exception.Message);
        }
    }
}