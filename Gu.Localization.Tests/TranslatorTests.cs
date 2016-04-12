namespace Gu.Localization.Tests
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Gu.Localization;
    using Gu.Localization.Tests.NoResources;

    using NUnit.Framework;

    public class TranslatorTests
    {
        [TestCase(null, "sv", "null")]
        [TestCase("Missing", "sv", "!Missing!")]
        [TestCase("EnglishOnly", "sv", "_EnglishOnly_")]
        [TestCase("AllLanguages", "it", "~AllLanguages~")]
        public void ErrorMessages(string key, string culture, string expected)
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            var actual = Translator.Translate<Properties.Resources>(key);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ErrorMessageWhenNoResources()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            var actual = Translator.Translate<ClassInNoResources>("Key");
            Assert.AreEqual("?Key?", actual);
        }

        [Test]
        public void TranslateLambda()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var actual = Translator.Translate(() => Properties.Resources.AllLanguages);

            Assert.AreEqual("English", actual);
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            actual = Translator.Translate(() => Properties.Resources.AllLanguages);
            Assert.AreEqual("Svenska", actual);
        }

        [Test]
        public void TranslateResourceManagerAndLambda()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var actual = Translator.Translate(Properties.Resources.ResourceManager, () => Properties.Resources.AllLanguages);

            Assert.AreEqual("English", actual);
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            actual = Translator.Translate(Properties.Resources.ResourceManager, () => Properties.Resources.AllLanguages);
            Assert.AreEqual("Svenska", actual);
        }

        [Test]
        public void TranslateResourceManagerAndName()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));

            Assert.AreEqual("English", actual);
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual("Svenska", actual);
        }

        [Test]
        public void TranslateTypeInAssemblyAndKey()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var actual = Translator.Translate<Properties.Resources>(nameof(Properties.Resources.AllLanguages));

            Assert.AreEqual("English", actual);
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            actual = Translator.Translate<Properties.Resources>(nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual("Svenska", actual);
        }

        [Test]
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