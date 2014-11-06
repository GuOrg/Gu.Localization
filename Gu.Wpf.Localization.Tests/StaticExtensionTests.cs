namespace Gu.Wpf.Localization.Tests
{
    using System;
    using System.Windows.Markup;
    using Moq;
    using NUnit.Framework;

    public class StaticExtensionTests
    {
        [Test]
        public void ProvideValueSharedDp()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var provideValueTargetMock = new Mock<IProvideValueTarget>();
            provideValueTargetMock.SetupGet(x => x.TargetObject).Returns(new SharedDp());
            serviceProviderMock.Setup(x => x.GetService(typeof(IProvideValueTarget)))
                                .Returns(provideValueTargetMock.Object);
            var translateExtension = new Gu.Wpf.Localization.StaticExtension("meh");
            var actual = translateExtension.ProvideValue(serviceProviderMock.Object);
            Assert.AreEqual("#meh#", actual);
        }
    }

    public class SharedDp
    {
    }
}
