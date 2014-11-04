//namespace Gu.Wpf.Localization.Tests
//{
//    using System;
//    using System.Globalization;
//    using System.Threading;
//    using System.Windows;
//    using System.Windows.Data;
//    using System.Windows.Markup;

//    using Gu.Wpf.Localization;

//    using Moq;

//    using NUnit.Framework;

//    public class TranslateExtensionTests
//    {
//        private Mock<IServiceProvider> _serviceProviderMock;

//        [SetUp]
//        public void Setup()
//        {
//            this._serviceProviderMock = new Mock<IServiceProvider>();
//            var provideValueTargetMock = new Mock<IProvideValueTarget>();
//            provideValueTargetMock.SetupGet(x => x.TargetObject).Returns(new DependencyObject());
//            _serviceProviderMock.Setup(x => x.GetService(typeof(IProvideValueTarget)))
//                                .Returns(provideValueTargetMock.Object);
//        }

//        [Explicit]
//        [TestCase("sv-SE", "MissingKey", "!MissingKey!")]
//        [TestCase("sv-SE", "NeutralOnly", "-NeutralOnly-")]
//        [TestCase("sv-SE", "EnglishOnly", "-EnglishOnly-")]
//        [TestCase("en-US", "EnglishOnly", "English")]
//        [TestCase("sv-SE", "AllLanguages", "Svenska")]
//        [TestCase("en-US", "AllLanguages", "English")]
//        public void ProvideValueRuntime(string cultureName, string key, string expected)
//        {
//            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
//            var translateExtension = new TranslateExtension(key);
//            DesignMode.OverrideIsInDesignMode = false;
//            var binding = (Binding)translateExtension.ProvideValue(_serviceProviderMock.Object);
//            var actual = ((TranslationData)binding.Source).Value;
//            Assert.AreEqual(expected, actual);
//        }

//        [Test]
//        public void ProvideValueSharedDp()
//        {
//            _serviceProviderMock = new Mock<IServiceProvider>();
//            var provideValueTargetMock = new Mock<IProvideValueTarget>();
//            provideValueTargetMock.SetupGet(x => x.TargetObject).Returns(new SharedDp());
//            _serviceProviderMock.Setup(x => x.GetService(typeof(IProvideValueTarget)))
//                                .Returns(provideValueTargetMock.Object);
//            var translateExtension = new TranslateExtension("meh");
//            var actual = translateExtension.ProvideValue(_serviceProviderMock.Object);
//            Assert.AreEqual(translateExtension, actual);
//        }
//    }

//    public class SharedDp
//    {
//    }
//}
