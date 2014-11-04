namespace Gu.Wpf.Localization.Tests
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
        private Mock<IServiceProvider> _serviceProviderMock;

        private Mock<IUriContext> _uriContextMock;

        [SetUp]
        public void Setup()
        {
            // http://stackoverflow.com/a/6005606/1069200
            string s = System.IO.Packaging.PackUriHelper.UriSchemePack;
            this._serviceProviderMock = new Mock<IServiceProvider>();
            this._uriContextMock = new Mock<IUriContext>();
            var uri = new Uri("pack://application:,,,/Gu.Wpf.Localization.Tests;component/controls/Meh.xaml", UriKind.Absolute);
            this._uriContextMock.SetupGet(x => x.BaseUri)
                                  .Returns(uri);
            this._serviceProviderMock.Setup(x => x.GetService(typeof(IUriContext)))
                               .Returns(this._uriContextMock.Object);
        }

        [TestCase("sv-SE", "MissingKey", "!MissingKey!")]
        [TestCase("sv-SE", "NeutralOnly", "-NeutralOnly-")]
        [TestCase("sv-SE", "EnglishOnly", "-EnglishOnly-")]
        [TestCase("en-US", "EnglishOnly", "English")]
        [TestCase("sv-SE", "AllLanguages", "Svenska")]
        [TestCase("en-US", "AllLanguages", "English")]
        public void ProvideValueRuntime(string cultureName, string key, string expected)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            var translateExtension = new TranslateExtension(key);
            DesignMode.OverrideIsInDesignMode = false;
            var binding = (Binding)translateExtension.ProvideValue(_serviceProviderMock.Object);
            var actual = ((TranslationData)binding.Source).Value;
            Assert.AreEqual(expected, actual);
        }
    }
}
