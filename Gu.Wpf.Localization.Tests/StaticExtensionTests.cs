namespace Gu.Wpf.Localization.Tests
{
    using System;
    using System.Windows.Markup;
    using Moq;
    using NUnit.Framework;

    public class StaticExtensionTests
    {
        [Test]
        public void GetAssemblyAndKey()
        {
            var typeResolverMock = new Mock<IXamlTypeResolver>();
            typeResolverMock.Setup(x => x.Resolve("p:Resources"))
                            .Returns(typeof(Properties.Resources));
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IXamlTypeResolver)))
                               .Returns(typeResolverMock.Object);
            var key = Gu.Wpf.Localization.StaticExtension.GetAssemblyAndKey(serviceProviderMock.Object, "p:Resources.Key");

            Assert.AreEqual(typeof(Properties.Resources).Assembly, key.Assembly);
            Assert.AreEqual("Key", key.Key);
        }
    }
}
