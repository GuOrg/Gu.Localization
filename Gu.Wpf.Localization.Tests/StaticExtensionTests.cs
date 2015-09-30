namespace Gu.Wpf.Localization.Tests
{
    using System;
    using System.Windows.Markup;

    using Gu.Wpf.Localization.Internals;

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
            var qnk = QualifiedNameAndKey.Parse("p:Resources.Key");
            var key = Gu.Wpf.Localization.StaticExtension.GetAssemblyAndKey(serviceProviderMock.Object, qnk);

            Assert.AreEqual(typeof(Properties.Resources).Assembly, key.Assembly);
            Assert.AreEqual("Key", key.Key);
        }
    }
}
