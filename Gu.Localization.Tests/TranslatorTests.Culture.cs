namespace Gu.Localization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using NUnit.Framework;

    public partial class TranslatorTests
    {
        public class Culture
        {
            [Test]
            [Explicit("Must be run separately due to static")]
            public void InitializesToCurrentCulture()
            {
                Assert.AreEqual(Translator.Culture.ThreeLetterISOLanguageName, CultureInfo.CurrentCulture.ThreeLetterISOLanguageName);
            }

            [Test]
            public void ChangeCultureToIllegalThrows()
            {
                Assert.Throws<ArgumentException>(() => Translator.Culture = CultureInfo.GetCultureInfo("it"));
            }

            [Test]
            public void SetCurrentCultureToNull()
            {
                Translator.Culture = null;
                Assert.AreEqual(null, Translator.Culture);
            }

            [Test]
            public void SetCurrentCultureToInvariant()
            {
                Translator.Culture = CultureInfo.InvariantCulture;
                Assert.AreEqual(CultureInfo.InvariantCulture, Translator.Culture);
            }

            [Test]
            public void ChangeCurrentCulture()
            {
                Translator.Culture = CultureInfo.GetCultureInfo("en");
                var changes = new List<CultureInfo>();
                var propertyChanges = new List<string>();
                Translator.StaticPropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                Translator.CurrentCultureChanged += (sender, info) => changes.Add(info.Culture);

                Translator.Culture = CultureInfo.GetCultureInfo("en");
                Assert.AreEqual(Translator.Cultures.Single(c => c.Name == "en"), Translator.Culture);
                Assert.AreSame(Translator.Cultures.Single(c => c.Name == "en"), Translator.CurrentCulture);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                Translator.Culture = CultureInfo.GetCultureInfo("sv");
                Assert.AreEqual(Translator.Cultures.Single(c => c.Name == "sv"), Translator.Culture);
                Assert.AreSame(Translator.Cultures.Single(c => c.Name == "sv"), Translator.CurrentCulture);
                CollectionAssert.AreEqual(new[] { "sv" }, changes.Select(x => x.TwoLetterISOLanguageName));
                CollectionAssert.AreEqual(new[] { "Culture", "CurrentCulture" }, propertyChanges);

                Translator.Culture = CultureInfo.GetCultureInfo("sv");
                Assert.AreEqual(Translator.Cultures.Single(c => c.Name == "sv"), Translator.Culture);
                Assert.AreSame(Translator.Cultures.Single(c => c.Name == "sv"), Translator.CurrentCulture);
                CollectionAssert.AreEqual(new[] { "sv" }, changes.Select(x => x.TwoLetterISOLanguageName));
                CollectionAssert.AreEqual(new[] { "Culture", "CurrentCulture" }, propertyChanges);

                Translator.Culture = CultureInfo.GetCultureInfo("en");
                Assert.AreEqual(Translator.Cultures.Single(c => c.Name == "en"), Translator.Culture);
                Assert.AreSame(Translator.Cultures.Single(c => c.Name == "en"), Translator.CurrentCulture);
                CollectionAssert.AreEqual(new[] { "sv", "en" }, changes.Select(x => x.TwoLetterISOLanguageName));
                CollectionAssert.AreEqual(new[] { "Culture", "CurrentCulture", "Culture", "CurrentCulture" }, propertyChanges);
            }

            [Test]
            public void Cultures()
            {
                var key = nameof(Properties.Resources.EnglishOnly);
                var italian = CultureInfo.GetCultureInfo("it");
                //// This call is for side effects to assert that 'it' is not added, YES NEEDS TO BE CALLED TWICE
                _ = Translator<Properties.Resources>.Translate(key, italian, ErrorHandling.ReturnErrorInfo);
                _ = Translator<Properties.Resources>.Translate(key, italian, ErrorHandling.ReturnErrorInfo);

                var cultures = Translator.Cultures.Select(x => x.Name)
                                                         .ToArray();
                CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, cultures);
            }
        }
    }
}
