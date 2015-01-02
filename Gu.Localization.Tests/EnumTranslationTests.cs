namespace Gu.Localization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Text;
    using NUnit.Framework;

    public class EnumTranslationTests
    {
        [Test]
        public void TranslateTest()
        {
            var translation = new EnumTranslation<DummyEnum>(Properties.Resources.ResourceManager, DummyEnum.AllLanguages);
            Translator.CurrentCulture = new CultureInfo("en");
            Assert.AreEqual("English", translation.Translated);

            var argses = new List<PropertyChangedEventArgs>();
            translation.PropertyChanged += (sender, args) => argses.Add(args);

            Translator.CurrentCulture = new CultureInfo("sv");
            Assert.AreEqual("Svenska", translation.Translated);
            Assert.AreEqual(1, argses.Count(x => x.PropertyName == ExpressionHelper.PropertyName(() => translation.Translated)));
        }

        [Test]
        public void MissingTranslations()
        {
            var translation = new EnumTranslation<DummyEnum>(Properties.Resources.ResourceManager, DummyEnum.AllLanguages);
            CollectionAssert.AreEqual(new[] { DummyEnum.Missing }, translation.MissingTranslations);
        }
    }

    public enum DummyEnum
    {
        AllLanguages,
        Missing
    }
}
