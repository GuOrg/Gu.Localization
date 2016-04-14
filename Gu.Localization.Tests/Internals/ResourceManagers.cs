namespace Gu.Localization.Tests.Internals
{
    using System;
    using System.Resources;

    using NUnit.Framework;

    public class ResourceManagersTests
    {
        [Test]
        public void ForTypeHappyPath()
        {
            var resourceManager1 = ResourceManagers.ForType(typeof(Properties.Resources));
            var resourceManager2 = ResourceManagers.ForType(typeof(Properties.Resources));
            Assert.AreSame(resourceManager1, resourceManager2);
            Assert.AreSame(resourceManager1, Properties.Resources.ResourceManager);
        }

        [Test]
        public void ForTypeThrows()
        {
            Assert.Throws<ArgumentException>(() => ResourceManagers.ForType(typeof(ResourceManager)));
        }

        [Test]
        public void TryGetForType()
        {
            ResourceManager resourceManager1;
            Assert.IsTrue(ResourceManagers.TryGetForType(typeof(Properties.Resources), out resourceManager1));
            ResourceManager resourceManager2;
            Assert.IsTrue(ResourceManagers.TryGetForType(typeof(Properties.Resources), out resourceManager2));
            Assert.AreSame(resourceManager1, resourceManager2);
            Assert.AreSame(resourceManager1, Properties.Resources.ResourceManager);
        }

        [Test]
        public void TryGetForTypeFails()
        {
            ResourceManager temp;
            Assert.IsFalse(ResourceManagers.TryGetForType(typeof(ResourceManager), out temp));
        }
    }
}
