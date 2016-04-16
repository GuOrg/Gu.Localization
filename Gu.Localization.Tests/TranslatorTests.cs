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
            var culture = cultureName != null
                                     ? CultureInfo.GetCultureInfo(cultureName)
                                     : CultureInfo.InvariantCulture;
            Translator.CurrentCulture = culture;
            var actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual(expected, actual);

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("it");
            actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), culture);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("en", 1, "Value: 1")]
        [TestCase("sv", 1, "Värde: 1")]
        [TestCase(null, 1, "Neutral: 1")]
        public void TranslateOneParameter(string cultureName, object arg, string expected)
        {
            var culture = cultureName != null
                                     ? CultureInfo.GetCultureInfo(cultureName)
                                     : CultureInfo.InvariantCulture;
            Translator.CurrentCulture = culture;
            var actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Value___0_), arg);
            Assert.AreEqual(expected, actual);

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("it");
            actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Value___0_), culture, arg);
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

        [Test]
        public void Cultures()
        {
            var cultures = Translator.Cultures.Select(x => x.Name).ToArray();
            CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, cultures);
        }
    }
}