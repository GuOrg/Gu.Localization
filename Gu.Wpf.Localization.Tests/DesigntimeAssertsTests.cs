namespace Gu.Wpf.Localization.Tests
{
    using System;
    using Moq;
    using NUnit.Framework;

    public class DesigntimeAssertsTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;
        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            DesignMode.OverrideIsInDesignMode = true;
        }

        [Test]
        public void AssertTranslationThrowsWhenKeyIsMissing()
        {
            Assert.Throws<ArgumentException>(() => DesignMode.AssertTranslation(_serviceProviderMock.Object, "MissingKey"));
        }

        [Test]
        public void DoesNotThrowWhenKeyIsPresent()
        {
            Assert.DoesNotThrow(() => DesignMode.AssertTranslation(_serviceProviderMock.Object, "AllLanguages"));
        }
    }
}
