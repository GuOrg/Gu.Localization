namespace Gu.Wpf.Localization.Tests
{
    using System.Globalization;
    using System.Threading;

    using Gu.Wpf.Localization;

    using NUnit.Framework;

    public class TranslationManagerTests
    {
        [Test]
        public void TranslateDefaultvalueTest()
        {
            TranslationManager.Instance.CurrentLanguage = CultureInfo.GetCultureInfo("no");
            var s1 = TranslationManager.Instance.Translate("AllLanguages");

            TranslationManager.Instance.CurrentLanguage = CultureInfo.GetCultureInfo("sv");
            var s2 = TranslationManager.Instance.Translate("AllLanguages");

            TranslationManager.Instance.CurrentLanguage = CultureInfo.GetCultureInfo("en");
            var s3 = TranslationManager.Instance.Translate("AllLanguages");

            Assert.AreEqual("Default", s1);
            Assert.AreEqual("Svenska", s2);
            Assert.AreEqual("English", s3);
        }

        [TestCase("MissingKey", "en", "-MissingKey-")]
        [TestCase("MissingKey", "no", "-MissingKey-")]
        public void TranslateNonExistingKeyTest(string key, string culture, string expected)
        {
            TranslationManager.Instance.CurrentLanguage = CultureInfo.GetCultureInfo("no");
            var actual = TranslationManager.Instance.Translate(key);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("sv", "Svenska")]
        [TestCase("en", "English")]
        public void TranslateDefaultLanguageTest(string cultureName, string expected)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            var actual = TranslationManager.Instance.Translate("AllLanguages");
            Assert.AreEqual(expected, actual);
        }
    }
}
