namespace Gu.Localization.Tests
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Gu.Localization;

    using NUnit.Framework;

    public class TranslatorTests
    {
        [TestCase("en", "English")]
        [TestCase("sv", "Svenska")]
        [TestCase(null, "So neutral")]
        public void TranslateResourceManagerAndNameHappyPath(string cultureName, string expected)
        {
            Translator.CurrentCulture = cultureName != null
                                            ? CultureInfo.GetCultureInfo(cultureName)
                                            : CultureInfo.InvariantCulture;
            var actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual(expected, actual);
        }

        [TestCase(null, "sv", "null")]
        [TestCase("Missing", "sv", "!Missing!")]
        [TestCase("EnglishOnly", "sv", "_EnglishOnly_")]
        [TestCase("EnglishOnly", "it", "~EnglishOnly~")]
        [TestCase("AllLanguages", "it", "So neutral")]
        public void ErrorMessages(string key, string culture, string expected)
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            var actual = Translator.Translate(Properties.Resources.ResourceManager, key);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NotifiesOnLanguageChanged()
        {
            var cultureInfos = new List<CultureInfo>();
            Translator.CurrentCultureChanged += (sender, info) => cultureInfos.Add(info);

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            CollectionAssert.AreEqual(new[] { "en" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            CollectionAssert.AreEqual(new[] { "en" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            CollectionAssert.AreEqual(new[] { "en", "sv" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));
        }
    }
}