namespace Gu.Localization.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;

    using NUnit.Framework;

    public class TranslatorGenericTests
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

        [TestCase(null, "sv", "null")]
        [TestCase("Missing", "sv", "!Missing!")]
        [TestCase("EnglishOnly", "sv", "_EnglishOnly_")]
        [TestCase("EnglishOnly", "it", "~EnglishOnly~")]
        [TestCase("AllLanguages", "it", "So neutral")]
        public void ErrorMessages(string key, string culture, string expected)
        {
            Translator.CurrentCulture = culture == null
                                            ? CultureInfo.InvariantCulture
                                            : CultureInfo.GetCultureInfo(culture);
            var actual = Translator<Properties.Resources>.Translate(key, ErrorHandling.ReturnInfo);
            Assert.AreEqual(expected, actual);

            Translator.ErrorHandling = ErrorHandling.ReturnInfo;
            actual = Translator<Properties.Resources>.Translate(key);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(null, "sv", "null")]
        [TestCase("Missing", "sv", "!Missing!")]
        [TestCase("EnglishOnly", "sv", "_EnglishOnly_")]
        [TestCase("EnglishOnly", "it", "~EnglishOnly~")]
        [TestCase("AllLanguages", "it", "So neutral")]
        public void Throws(string key, string culture, string expected)
        {
            Translator.CurrentCulture = culture == null
                                            ? CultureInfo.InvariantCulture
                                            : CultureInfo.GetCultureInfo(culture);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(key, ErrorHandling.Default));
            Assert.AreEqual("", exception.Message);

            exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(key, ErrorHandling.Throw));
            Assert.AreEqual("", exception.Message);
        }
    }
}