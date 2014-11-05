namespace Gu.Wpf.Localization.Tests
{
    using System;
    using System.Windows.Markup;

    using Gu.Wpf.Localization;

    using Moq;

    using NUnit.Framework;

    public class TranslateExtensionTests
    {
        [Test]
        public void ProvideValueSharedDp()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var provideValueTargetMock = new Mock<IProvideValueTarget>();
            provideValueTargetMock.SetupGet(x => x.TargetObject).Returns(new SharedDp());
            serviceProviderMock.Setup(x => x.GetService(typeof(IProvideValueTarget)))
                                .Returns(provideValueTargetMock.Object);
            var translateExtension = new ResourceExtension("meh");
            var actual = translateExtension.ProvideValue(serviceProviderMock.Object);
            Assert.AreEqual("#meh#", actual);
        }
    }

    public class SharedDp
    {
    }
}
