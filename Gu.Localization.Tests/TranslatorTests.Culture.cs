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
            [Test, Explicit("Must be run separately due to static")]
            public void InitializesToCurrentUiCulture()
            {
                Assert.AreEqual(Translator.CurrentCulture.ThreeLetterISOLanguageName, CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName);
            }

            [Test]
            public void ChangeCultureToIllegalThrows()
            {
                Assert.Throws<ArgumentException>(() => Translator.CurrentCulture = CultureInfo.GetCultureInfo("it"));
            }

            [Test]
            public void SetCurrentCultureToNull()
            {
                Translator.CurrentCulture = null;
                Assert.AreEqual(null, Translator.CurrentCulture);
            }

            [Test]
            public void SetCurrentCultureToInvariant()
            {
                Translator.CurrentCulture = CultureInfo.InvariantCulture;
                Assert.AreEqual(CultureInfo.InvariantCulture, Translator.CurrentCulture);
            }

            [Test]
            public void ChangeCurrentCulture()
            {
                var changes = new List<CultureInfo>();
                Translator.CurrentCultureChanged += (sender, info) => changes.Add(info);

                Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
                Assert.AreSame(Translator.Cultures.Single(c => c.Name == "en"), Translator.CurrentCulture);
                CollectionAssert.AreEqual(new[] { "en" }, changes.Select(x => x.TwoLetterISOLanguageName));

                Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
                Assert.AreSame(Translator.Cultures.Single(c => c.Name == "en"), Translator.CurrentCulture);
                Assert.AreEqual(1, changes.Count);

                Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
                Assert.AreSame(Translator.Cultures.Single(c => c.Name == "sv"), Translator.CurrentCulture);
                CollectionAssert.AreEqual(new[] { "en", "sv" }, changes.Select(x => x.TwoLetterISOLanguageName));
            }

            [Test]
            public void Cultures()
            {
                var key = nameof(Properties.Resources.EnglishOnly);
                var italian = CultureInfo.GetCultureInfo("it");
                // This call is for side effects to assert that 'it' is not added, YES NEEDS TO BE CALLED TWICE
                Translator<Properties.Resources>.Translate(key, italian, ErrorHandling.ReturnErrorInfo);
                Translator<Properties.Resources>.Translate(key, italian, ErrorHandling.ReturnErrorInfo);

                var cultures = Translator.Cultures.Select(x => x.Name)
                                         .ToArray();
                CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, cultures);
            }
        }
    }
}