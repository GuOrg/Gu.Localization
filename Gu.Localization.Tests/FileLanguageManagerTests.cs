namespace Gu.Localization.Tests
{
    using System.Globalization;
    using System.Linq;

    using NUnit.Framework;

    public class FileLanguageManagerTests
    {
        [TestCase("en", "English")]
        [TestCase("sv", "Svenska")]
        [TestCase("no", "So neutral")]
        public void Translate(string cultureName, string expected)
        {
            var manager = new FileLanguageManager.FileLanguageManagerFactory().GetOrCreate(GetType());
            var cultureInfo = new CultureInfo(cultureName);
            var translated = manager.Translate(cultureInfo, nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual(expected, translated);
        }

        [Test]
        public void Languages()
        {
            var manager = new FileLanguageManager.FileLanguageManagerFactory().GetOrCreate(GetType());
            var expected = new[] { "de", "en", "sv" };
            var actual = manager.Languages.Select(x => x.TwoLetterISOLanguageName)
                                .ToArray();
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}