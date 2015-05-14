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
            var translator = new Translator(new ResourceManagerWrapper(Properties.Resources.ResourceManager));
            Translator.CurrentCulture = cultureInfo;
            var actual = translator.Translate(key);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TranslateFromCodeBehindTest()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var actual = Translator.Translate(Properties.Resources.ResourceManager, () => Properties.Resources.AllLanguages);

            Assert.AreEqual("English", actual);
            var allLanguages = Properties.Resources.AllLanguages;
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            actual = Translator.Translate(Properties.Resources.ResourceManager, () => Properties.Resources.AllLanguages);
            allLanguages = Properties.Resources.AllLanguages;
            Assert.AreEqual("Svenska", actual);
        }

        [Test, Explicit("Static event, ew")]
        public void NotifiesOnLanguageChanged()
        {
            var cultureInfos = new List<CultureInfo>();
            Translator.LanguageChanged += (sender, info) => cultureInfos.Add(info);

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            CollectionAssert.AreEqual(new[] { "en" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            CollectionAssert.AreEqual(new[] { "en" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            CollectionAssert.AreEqual(new[] { "en", "sv" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));
        }
    }
}