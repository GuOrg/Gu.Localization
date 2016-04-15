namespace Gu.Localization.Tests.Errors
{
    using System.Resources;
    using Gu.Localization.Errors;
    using Moq;
    using NUnit.Framework;

    public class ResourceManagerErrorsTests
    {
        [Test]
        public void TestNameTest()
        {
            var resourceManagerMock = new Mock<ResourceManager>();
            var errors = ResourceManagerErrors.For(resourceManagerMock.Object);
            Assert.Inconclusive("Implement");
        }

        [Test]
        public void MissingTranslations()
        {
            var missingTranslations = ResourceManagerErrors.For(Properties.Resources.ResourceManager);
            //CollectionAssert.AreEqual(new[] { DummyEnum.Missing }, missingTranslations);
            Assert.Inconclusive("Implement");
        }
    }
}
