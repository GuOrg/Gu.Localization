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

        [Test]
        public void GetAssemblyAndKey()
        {
            var typeResolverMock = new Mock<IXamlTypeResolver>();
            typeResolverMock.Setup(x => x.Resolve("p:Resources"))
                            .Returns(typeof(Properties.Resources));
            var key = Gu.Wpf.Localization.StaticExtension.GetAssemblyAndKey(typeResolverMock.Object, "p:Resources.Key");
            Assert.AreEqual(typeof(Properties.Resources).Assembly, key.Assembly);
            Assert.AreEqual("Key", key.Key);
        }
    }

    public class SharedDp
    {
    }
}
