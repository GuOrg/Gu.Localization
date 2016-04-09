namespace Gu.Localization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using Moq;
    using NUnit.Framework;

    public class TranslationTests
    {
        [Test]
        public void Ctor()
        {
            var translation = new Translation(() => Properties.Resources.AllLanguages);
            Translator.CurrentCulture = new CultureInfo("en");
            Assert.AreEqual("English", translation.Translated);
        }

        [Test]
        public void NotifiesAndTranslatesWhenLanguageChanges()
        {
            Translator.CurrentCulture = new CultureInfo("en");
            var translation = new Translation(() => Properties.Resources.AllLanguages);
            var argses = new List<PropertyChangedEventArgs>();
            translation.PropertyChanged += (sender, args) => argses.Add(args);

            Assert.AreEqual("English", translation.Translated);

            Translator.CurrentCulture = new CultureInfo("sv");
            Assert.AreEqual(1, argses.Count(x => x.PropertyName == "Translated"));
            Assert.AreEqual("Svenska", translation.Translated);
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
            var actual = translator.Translated;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NotifiesAndTranslatesObservable()
        {
            IObserver<object> observer = null;
            var mock = new Mock<IObservable<object>>();
            mock.Setup(x => x.Subscribe(It.IsAny<IObserver<object>>()))
                .Returns((IDisposable) null)
                .Callback<IObserver<object>>(x => observer = x);
            this.DummyProperty = "Missing";
            var translation = new Translation(Properties.Resources.ResourceManager, () => this.DummyProperty, mock.Object);
            Translator.CurrentCulture = new CultureInfo("en");

            var argses = new List<PropertyChangedEventArgs>();
            translation.PropertyChanged += (sender, args) => argses.Add(args);

            Assert.AreEqual("!Missing!", translation.Translated);
            Assert.AreEqual(0, argses.Count(x => x.PropertyName == "Translated"));

            this.DummyProperty = "AllLanguages";
            Assert.AreEqual("English", translation.Translated);
            Assert.AreEqual(0, argses.Count(x => x.PropertyName == "Translated"));

            observer.OnNext(null);
            Assert.AreEqual("English", translation.Translated);
            Assert.AreEqual(1, argses.Count(x => x.PropertyName == "Translated"));
        }

        public string DummyProperty { get; private set; }
    }
}
