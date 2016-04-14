namespace Gu.Localization.Tests
{
    using System.Globalization;

    using NUnit.Framework;

    public class TranslatorGenericTests
    {
        [Test]
        public void HappyPath()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var actual = Translator<Properties.Resources>.Translate(nameof(Properties.Resources.AllLanguages));

            Assert.AreEqual("English", actual);
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            actual = Translator<Properties.Resources>.Translate(nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual("Svenska", actual);
        }

        [TestCase(null, "sv", "null")]
        [TestCase("Missing", "sv", "!Missing!")]
        [TestCase("EnglishOnly", "sv", "_EnglishOnly_")]
        [TestCase("EnglishOnly", "it", "~EnglishOnly~")]
        [TestCase("AllLanguages", "it", "So neutral")]
        public void ErrorMessages(string key, string culture, string expected)
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            var actual = Translator<Properties.Resources>.Translate(key);
            Assert.AreEqual(expected, actual);
        }
    }
}