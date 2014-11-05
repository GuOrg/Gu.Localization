namespace Gu.Wpf.Localization.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

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
            var translator = new Translator(new ResourceManagerWrapper(Properties.Resources.ResourceManager), key);
            Translator.CurrentCulture = cultureInfo;
            var actual = translator.Value;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Notifies()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var translator = new Translator(new ResourceManagerWrapper(Properties.Resources.ResourceManager), "AllLanguages");
            var list = new List<string>();
            translator.PropertyChanged += (sender, args) => list.Add(args.PropertyName);
            Assert.AreEqual("English", translator.Value);
            CollectionAssert.IsEmpty(list);

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            Assert.AreEqual("English", translator.Value);
            CollectionAssert.IsEmpty(list);

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual("Svenska", translator.Value);
            CollectionAssert.AreEqual(new[] { "Value" }, list);
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

        [Test]
        public void NotifiesOnLanguageChanged()
        {
            var cultureInfos = new List<CultureInfo>();
            Translator.LanguageCahnged += (sender, info) => cultureInfos.Add(info);

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            CollectionAssert.AreEqual(new[] { "en" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            CollectionAssert.AreEqual(new[] { "en" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));

            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            CollectionAssert.AreEqual(new[] { "en", "sv" }, cultureInfos.Select(x => x.TwoLetterISOLanguageName));
        }
    }
}