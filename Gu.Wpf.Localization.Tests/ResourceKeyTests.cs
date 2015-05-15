namespace Gu.Wpf.Localization.Tests
{
    using System.Windows.Markup;

    using Gu.Wpf.Localization.Internals;

    using Moq;
    using NUnit.Framework;

    public class ResourceKeyTests
    {
        [Test]
        public void HappyPath()
        {
            var typeResolverMock = new Mock<IXamlTypeResolver>();
            TypeNameAndKey typeNameAndKey;
            Assert.IsTrue(TypeNameAndKey.TryParse("p:Resources.Key", out typeNameAndKey));
            typeResolverMock.Setup(x => x.Resolve("p:Resources")).Returns(typeof(Properties.Resources));
            var resourceKey = ResourceKey.Resolve(typeNameAndKey, typeResolverMock.Object, true);
            Assert.AreEqual(Properties.Resources.ResourceManager, resourceKey.ResourceManager);
            Assert.AreEqual("Key", resourceKey.Key);
        }
    }
}
