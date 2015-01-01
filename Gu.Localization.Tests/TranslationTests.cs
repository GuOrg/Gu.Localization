namespace Gu.Localization.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    using NUnit.Framework;

    public class TranslationTests
    {
        [Test]
        public void Ctor()
        {
            var translation = new Translation(() => Properties.Resources.AllLanguages);
            Translator.CurrentCulture = new CultureInfo("en");
            Assert.AreEqual("English", translation.Value);
        }

        [Test]
        public void NotifiesAndTranslatesWhenLanguageChanges()
        {
            Translator.CurrentCulture = new CultureInfo("en");
            var translation = new Translation(() => Properties.Resources.AllLanguages);
            var argses = new List<PropertyChangedEventArgs>();
            translation.PropertyChanged += (sender, args) => argses.Add(args);

            Assert.AreEqual("English", translation.Value);

            Translator.CurrentCulture = new CultureInfo("sv");
            Assert.AreEqual(1, argses.Count(x => x.PropertyName == "Value"));
            Assert.AreEqual("Svenska", translation.Value);
        }

        [TestCase("AllLanguages", "en", "English")]
        [TestCase("AllLanguages", "sv", "Svenska")]
        [TestCase("Missing", "sv", "!Missing!")]
        [TestCase("EnglishOnly", "sv", "")]
        public void Translate(string key, string culture, string expected)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            var translator = new Translation(Properties.Resources.ResourceManager, key);
            Translator.CurrentCulture = cultureInfo;
            var actual = translator.Value;
            Assert.AreEqual(expected, actual);
        }
    }
}
