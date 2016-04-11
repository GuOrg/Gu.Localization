namespace Gu.Wpf.Localization.Tests
{
    using System.Linq;

    using Gu.Localization;

    using NUnit.Framework;

    [RequiresSTA]
    public class LanguageSelectorTests
    {
        [Test]
        public void TracksLanguage()
        {
            Assert.Inconclusive("Requires dispatcher, UI-test it");
            var selector = new LanguageSelector { AutogenerateLanguages = true };
            CollectionAssert.AreEqual(Translator.AllCultures, selector.Languages.Select(x => x.Culture));
            Assert.AreEqual(Translator.CurrentCulture, selector.Languages.Single(x => x.IsSelected).Culture);

            Translator.CurrentCulture = Translator.AllCultures[1];
            Assert.AreEqual(Translator.CurrentCulture, selector.Languages.Single(x => x.IsSelected).Culture);

            Translator.CurrentCulture = Translator.AllCultures[0];
            Assert.AreEqual(Translator.CurrentCulture, selector.Languages.Single(x => x.IsSelected).Culture);
        }
    }
}
