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

        [Test]
        public void TranslateNonExistingKeyTest()
        {
            TranslationManager.Instance.CurrentLanguage = CultureInfo.GetCultureInfo("no");
            var s1 = TranslationManager.Instance.Translate("NoEntry");

            TranslationManager.Instance.CurrentLanguage = CultureInfo.GetCultureInfo("sv");
            var s2 = TranslationManager.Instance.Translate("NoEntry");

            TranslationManager.Instance.CurrentLanguage = CultureInfo.GetCultureInfo("en");
            var s3 = TranslationManager.Instance.Translate("NoEntry");

            Assert.AreEqual("!NoEntry!", s1);
            Assert.AreEqual("!NoEntry!", s2);
            Assert.AreEqual("!NoEntry!", s3);
        }

        [TestCase("sv-SE", "Svenska")]
        [TestCase("en-US", "English")]
        public void TranslateDefaultLanguageTest(string cultureName, string expected)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            var actual = TranslationManager.Instance.Translate("AllLanguages");
            Assert.AreEqual(expected, actual);
        }
    }
}
