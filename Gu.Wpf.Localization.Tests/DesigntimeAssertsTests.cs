namespace Gu.Wpf.Localization.Tests
{
    using System;
    using System.Windows;
    using System.Windows.Markup;

    using Moq;
    using NUnit.Framework;

    public class DesigntimeAssertsTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;
        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            var provideValueTargetMock = new Mock<IProvideValueTarget>();
            _serviceProviderMock.Setup(x => x.GetService(typeof(IProvideValueTarget)))
                                .Returns(provideValueTargetMock.Object);
            DesignMode.OverrideIsInDesignMode = true;
        }

        [Test]
        public void AssertTranslationThrowsWhenKeyIsMissing()
        {
            Assert.Throws<ArgumentException>(() => DesignMode.AssertTranslation(_serviceProviderMock.Object, "NotAKey"));
        }

        [Test]
        public void DoesNotThrowWhenKeyIsPresent()
        {
            Assert.DoesNotThrow(() => DesignMode.AssertTranslation(_serviceProviderMock.Object, "AllLanguages"));
        }
    }
}
