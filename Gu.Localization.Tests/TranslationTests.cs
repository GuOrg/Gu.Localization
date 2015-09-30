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
        public void GetOrCreateLambda()
        {
            var translation = Translation.GetOrCreate(() => Properties.Resources.AllLanguages);
            var translation2 = Translation.GetOrCreate(() => Properties.Resources.AllLanguages);
            Assert.AreSame(translation, translation2);
        }

        [Test]
        public void GetOrCreateTypeAndString()
        {
            var translation = Translation.GetOrCreate(GetType(), nameof(Properties.Resources.AllLanguages));
            var translation2 = Translation.GetOrCreate(GetType(), nameof(Properties.Resources.AllLanguages));
            Assert.AreSame(translation, translation2);
        }

        [Test]
        public void NotifiesAndTranslatesWhenLanguageChanges()
        {
            Translator.CurrentCulture = new CultureInfo("en");
            var translation = Translation.GetOrCreate(() => Properties.Resources.AllLanguages);
            var argses = new List<PropertyChangedEventArgs>();
            translation.PropertyChanged += (sender, args) => argses.Add(args);

            Assert.AreEqual("English", translation.Translated);

            Translator.CurrentCulture = new CultureInfo("sv");
            Assert.AreEqual(1, argses.Count(x => x.PropertyName == nameof(translation.Translated)));
            Assert.AreEqual("Svenska", translation.Translated);
        }

        [TestCase("AllLanguages", "en", "English")]
        [TestCase("AllLanguages", "sv", "Svenska")]
        [TestCase("Missing", "sv", "!Missing!")]
        [TestCase("EnglishOnly", "sv", "")]
        public void Translate(string key, string culture, string expected)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            var translator = Translation.GetOrCreate(GetType(), key);
            Translator.CurrentCulture = cultureInfo;
            var actual = translator.Translated;
            Assert.AreEqual(expected, actual);
        }
    }
}
