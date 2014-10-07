namespace GULocalization.Tests
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Wpf.Localization;

    using Moq;
    using NUnit.Framework;
    public class TranslateExtensionTests
    {
        [SetUp]
        public void Setup()
        {
            // http://stackoverflow.com/a/6005606/1069200
            string s = System.IO.Packaging.PackUriHelper.UriSchemePack;
        }

        [TestCase("sv-SE", "Teststring1 Svenska")]
        [TestCase("en-US", "Teststring1 English")]
        public void ProvideValue(string cultureName, string expected)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            var translateExtension = new TranslateExtension("Teststring1");
            var serviceProviderMock = new Mock<IServiceProvider>();
            var uriContextMock = new Mock<IUriContext>();
            var uri = new Uri("pack://application:,,,/Gu.Wpf.Localization.Tests;component/controls/Meh.xaml", UriKind.Absolute);
            uriContextMock.SetupGet(x => x.BaseUri)
                                  .Returns(uri);
            serviceProviderMock.Setup(x => x.GetService(typeof(IUriContext)))
                               .Returns(uriContextMock.Object);
            var actual = (Binding)translateExtension.ProvideValue(serviceProviderMock.Object);
            Assert.AreEqual(expected, ((TranslationData)actual.Source).Value);
        }

        [TestCase("Teststring1", null)]
        [TestCase("No translation for this", "Translation for: 'No translation for this' is missing in {en, sv}")]
        [TestCase("EnglishOnly", "Translation for: 'EnglishOnly' is missing in {sv}")]
        public void AssertTranslation(string key, string expected)
        {
            if (expected == null)
            {
                new TranslateExtension("Meh").AssertTranslation(key, TranslationManager.Instance);
            }
            else
            {
                var exception = Assert.Throws<Exception>(() => new TranslateExtension("meh").AssertTranslation(key, TranslationManager.Instance));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
