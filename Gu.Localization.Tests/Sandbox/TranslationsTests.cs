namespace Gu.Localization.Tests.Sandbox
{
    using System.Linq;

    using NUnit.Framework;

    public class TranslationsTests
    {
        [Test]
        public void GetOrCreate()
        {
            var resourceManager = Properties.Resources.ResourceManager;
            var translations = Translations.GetOrCreate(this.GetType().Assembly);
            Assert.AreEqual(Properties.Resources.ResourceManager.BaseName, translations.BaseName);
            CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, translations.Cultures.Select(x => x.TwoLetterISOLanguageName));
        }
    }
}