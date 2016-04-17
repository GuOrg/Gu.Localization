namespace Gu.Localization.Tests
{
    using System;
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

        [TestCase(nameof(Properties.Resources.NeutralOnly), "sv", "So neutral")]
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

            foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling)).OfType<ErrorHandling>())
            {
                actual = Translator.Translate(Properties.Resources.ResourceManager, key, errorHandling);
                Assert.AreEqual(expected, actual);
            }

            foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling)).OfType<ErrorHandling>())
            {
                Translator.ErrorHandling = errorHandling;
                actual = Translator.Translate(Properties.Resources.ResourceManager, key);
                Assert.AreEqual(expected, actual);
            }
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
        [TestCase(nameof(Properties.Resources.AllLanguages), "it", "The resourcemanager Gu.Localization.Tests.Properties.Resources does not have a translations for the culture: it\r\nParameter name: culture")]
        [TestCase(nameof(Properties.Resources.NeutralOnly), "it", "The resourcemanager Gu.Localization.Tests.Properties.Resources does not have a translation to the culture: it for the key: NeutralOnly\r\nParameter name: culture, key")]
        public void Throws(string key, string culture, string expected)
        {
            Translator.CurrentCulture = culture == null
                                            ? CultureInfo.InvariantCulture
                                            : CultureInfo.GetCultureInfo(culture);
            Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, key, ErrorHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Translator.ErrorHandling = ErrorHandling.Throw;
            exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, key));
            Assert.AreEqual(expected, exception.Message);

            exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, key, ErrorHandling.Default));
            Assert.AreEqual(expected, exception.Message);

            exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, key, ErrorHandling.Throw));
            Assert.AreEqual(expected, exception.Message);
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
            var key = nameof(Properties.Resources.EnglishOnly);
            var italian = CultureInfo.GetCultureInfo("it");
            // This call is for side effects to assert that 'it' is not added, YES NEEDS TO BE CALLED TWICE
            Translator<Properties.Resources>.Translate(key, italian, ErrorHandling.ReturnErrorInfo);
            Translator<Properties.Resources>.Translate(key, italian, ErrorHandling.ReturnErrorInfo);

            var cultures = Translator.Cultures.Select(x => x.Name).ToArray();
            CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, cultures);
        }
    }
}