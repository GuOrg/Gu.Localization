namespace Gu.Wpf.Localization.Tests
{
    using System.Windows.Markup;
    using Moq;
    using NUnit.Framework;

    public class ResourceKeyTests
    {
        [Test]
        public void HappyPath()
        {
            var typeResolverMock = new Mock<IXamlTypeResolver>();
            typeResolverMock.Setup(x => x.Resolve("p:Resources")).Returns(typeof(Properties.Resources));
            var resourceKey = new ResourceKey("p:Resources.Key", typeResolverMock.Object, true);
            Assert.AreEqual(Properties.Resources.ResourceManager, resourceKey.ResourceManager);
            Assert.AreEqual("Key", resourceKey.Key);
        }
    }
}
