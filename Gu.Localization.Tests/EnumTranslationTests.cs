namespace Gu.Localization.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    using Gu.Localization.Tests.Errors;

    using NUnit.Framework;

    public class EnumTranslationTests
    {
        [Test]
        public void Create()
        {
            var translation = EnumTranslation.Create(Properties.Resources.ResourceManager, DummyEnum.AllLanguages, ErrorHandling.ReturnErrorInfo);
            Translator.Culture = CultureInfo.GetCultureInfo("en");
            Assert.AreEqual("English", translation.Translated);

            var argses = new List<PropertyChangedEventArgs>();
            translation.PropertyChanged += (sender, args) => argses.Add(args);

            Translator.Culture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual("Svenska", translation.Translated);
            Assert.AreEqual(1, argses.Count(x => x.PropertyName == nameof(translation.Translated)));
        }
    }
}
