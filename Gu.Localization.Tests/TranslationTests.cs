// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.Localization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using NUnit.Framework;

    public class TranslationTests
    {
        [Test]
        public void GetOrCreateResourceManagerAndKeyCaches1()
        {
            Translator.Culture = new CultureInfo("sv");
            var translation1 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            var translation2 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            Assert.AreSame(translation1, translation2);
        }

        [Test]
        public void GetOrCreateResourceManagerAndKeyCaches2()
        {
            Translator.Culture = new CultureInfo("sv");
            var translation1 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), ErrorHandling.ReturnErrorInfo);
            var translation2 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), ErrorHandling.ReturnErrorInfo);
            Assert.AreSame(translation1, translation2);
            Assert.AreEqual(ErrorHandling.ReturnErrorInfo, translation1.ErrorHandling);

            Translator.ErrorHandling = ErrorHandling.Throw;
            var translation3 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), ErrorHandling.Inherit);
            Assert.AreNotSame(translation1, translation3);
            Assert.AreEqual(ErrorHandling.Throw, translation3.ErrorHandling);

            var translation4 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), ErrorHandling.Throw);
            Assert.AreNotSame(translation1, translation4);
            Assert.AreSame(translation3, translation4);
            Assert.AreEqual(ErrorHandling.Throw, translation4.ErrorHandling);
        }

        [Test]
        public void GetOrCreateThrowsForMissing()
        {
            Translator.Culture = new CultureInfo("sv");
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translation.GetOrCreate(Properties.Resources.ResourceManager, "Missing", ErrorHandling.Throw));
            var expected = "The ResourceManager: Gu.Localization.Tests.Properties.Resources does not have the key: Missing\r\n" +
                           "Parameter name: key";
            Assert.AreEqual(expected, exception.Message);

            Translator.ErrorHandling = ErrorHandling.Throw;
            exception = Assert.Throws<ArgumentOutOfRangeException>(() => Translation.GetOrCreate(Properties.Resources.ResourceManager, "Missing", ErrorHandling.Inherit));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void GetOrCreateReturnsStaticIfMissing()
        {
            Translator.Culture = new CultureInfo("sv");
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, "Missing", ErrorHandling.ReturnErrorInfo);
            Assert.IsInstanceOf<StaticTranslation>(translation);
            Assert.AreEqual("!Missing!", translation.Translated);
            Assert.AreEqual("!Missing!", translation.Translate(CultureInfo.GetCultureInfo("it")));
            Assert.AreEqual("Missing", translation.Key);
            Assert.AreEqual(ErrorHandling.ReturnErrorInfo, translation.ErrorHandling);

            Translator.ErrorHandling = ErrorHandling.ReturnErrorInfoPreserveNeutral;
            translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, "Missing", ErrorHandling.Inherit);
            Assert.IsInstanceOf<StaticTranslation>(translation);
            Assert.AreEqual("!Missing!", translation.Translated);
            Assert.AreEqual("!Missing!", translation.Translate(CultureInfo.GetCultureInfo("it")));
            Assert.AreEqual("Missing", translation.Key);
            Assert.AreEqual(ErrorHandling.ReturnErrorInfoPreserveNeutral, translation.ErrorHandling);
        }

        [Test]
        public void NotifiesAndTranslatesWhenLanguageChanges()
        {
            Translator.Culture = new CultureInfo("sv");
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            var changes = new List<string>();
            translation.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

            Translator.Culture = new CultureInfo("en");
            Assert.AreEqual("English", translation.Translated);
            CollectionAssert.AreEqual(new[] { nameof(Translation.Translated) }, changes);

            Translator.Culture = new CultureInfo("sv");
            Assert.AreEqual("Svenska", translation.Translated);
            CollectionAssert.AreEqual(new[] { nameof(Translation.Translated), nameof(Translation.Translated) }, changes);
        }

        [TestCase(nameof(Properties.Resources.AllLanguages), "en", "English")]
        [TestCase(nameof(Properties.Resources.AllLanguages), "sv", "Svenska")]
        [TestCase(nameof(Properties.Resources.EnglishOnly), "sv", "_EnglishOnly_")]
        public void Translate(string key, string culture, string expected)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, key, ErrorHandling.ReturnErrorInfo);
            Translator.Culture = cultureInfo;
            var actual = translation.Translated;
            Assert.AreEqual(expected, actual);
        }
    }
}
