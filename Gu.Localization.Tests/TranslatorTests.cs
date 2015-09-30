namespace Gu.Localization.Tests
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Gu.Localization;

    using NUnit.Framework;

    public class TranslatorTests
    {
        [TestCase("AllLanguages", "en", "English")]
        [TestCase("AllLanguages", "sv", "Svenska")]
        [TestCase("Missing", "sv", "!Missing!")]
        [TestCase("EnglishOnly", "sv", "")]
        public void Translate(string key, string culture, string expected)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            Translator.CurrentCulture = cultureInfo;
            var actual = Translator.Translate(GetType(), key);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("en-US", "English")]
        [TestCase("sv-SE", "Svenska")]
        [TestCase("nb-NO", "So neutral")]
        public void Translate(string cultureName, string expected)
        {
            Translator.CurrentCulture = new CultureInfo(cultureName);
            var translated = Translator.Translate(() => Properties.Resources.AllLanguages);
            Assert.AreEqual(expected, translated);
        }

        [Test]
        public void TranslateFromCodeBehindLambda()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var actual = Translator.Translate(() => Properties.Resources.AllLanguages);
            Assert.AreEqual("English", actual);
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            actual = Translator.Translate(() => Properties.Resources.AllLanguages);
            Assert.AreEqual("Svenska", actual);
        }

        [Test]
        public void TranslateFromCodeBehindTypeAndKey()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var actual = Translator.Translate(GetType(), nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual("English", actual);
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            actual = Translator.Translate(GetType(), nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual("Svenska", actual);
        }

        [Test, Explicit("Static event, ew")]
        public void NotifiesOnLanguageChanged()
        {
            var cultureInfos = new List<CultureInfo>();
            Translator.CurrentLanguageChanged += (sender, info) => cultureInfos.Add(info);

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            CollectionAssert.AreEqual(new[] { "en" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            CollectionAssert.AreEqual(new[] { "en" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            CollectionAssert.AreEqual(new[] { "en", "sv" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));
        }
    }
}