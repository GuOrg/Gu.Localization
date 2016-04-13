namespace Gu.Localization.Tests
{
    using System.Collections.Generic;
    using System.Globalization;

    using NUnit.Framework;

    public class TranslationTests
    {
        [Test]
        public void GetOrCreateResourceManagerAndKey()
        {
            Translator.CurrentCulture = new CultureInfo("sv");
            var translation1 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            var translation2 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            Assert.AreSame(translation1, translation2);
        }

        [Test]
        public void NotifiesAndTranslatesWhenLanguageChanges()
        {
            Translator.CurrentCulture = new CultureInfo("sv");
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            var changes = new List<string>();
            translation.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

            Translator.CurrentCulture = new CultureInfo("en");
            Assert.AreEqual("English", translation.Translated);
            CollectionAssert.AreEqual(new[] { nameof(Translation.Translated) }, changes);

            Translator.CurrentCulture = new CultureInfo("sv");
            Assert.AreEqual("Svenska", translation.Translated);
            CollectionAssert.AreEqual(new[] { nameof(Translation.Translated), nameof(Translation.Translated) }, changes);
        }

        [TestCase("AllLanguages", "en", "English")]
        [TestCase("AllLanguages", "sv", "Svenska")]
        [TestCase("Missing", "sv", "!Missing!")]
        [TestCase("EnglishOnly", "sv", "")]
        public void Translate(string key, string culture, string expected)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            var translator = Translation.GetOrCreate(Properties.Resources.ResourceManager, key);
            Translator.CurrentCulture = cultureInfo;
            var actual = translator.Translated;
            Assert.AreEqual(expected, actual);
        }
    }
}
