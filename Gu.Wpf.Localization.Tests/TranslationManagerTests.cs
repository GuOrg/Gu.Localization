namespace GULocalization.Tests
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
            var s1 = TranslationManager.Instance.Translate("Teststring1");

            TranslationManager.Instance.CurrentLanguage = CultureInfo.GetCultureInfo("sv");
            var s2 = TranslationManager.Instance.Translate("Teststring1");

            TranslationManager.Instance.CurrentLanguage = CultureInfo.GetCultureInfo("en");
            var s3 = TranslationManager.Instance.Translate("Teststring1");

            Assert.AreEqual("Teststring1 Default", s1);
            Assert.AreEqual("Teststring1 Svenska", s2);
            Assert.AreEqual("Teststring1 English", s3);
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

        [TestCase("sv-SE", "Teststring1 Svenska")]
        [TestCase("en-US", "Teststring1 English")]
        public void TranslateDefaultLanguageTest(string cultureName, string expected)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            var actual = TranslationManager.Instance.Translate("Teststring1");
            Assert.AreEqual(expected, actual);
        }
    }
}
