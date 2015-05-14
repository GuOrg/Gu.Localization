namespace Gu.Localization.Tests
{
    using System.Linq;

    using Gu.Localization;

    using NUnit.Framework;

    public class ResourceManagerWrapperTests
    {
        [Test]
        public void Languages()
        {
            var resourceManagerWrapper = new ResourceManagerWrapper(Properties.Resources.ResourceManager);
            var expected = new[] { "de", "en", "sv" };
            CollectionAssert.AreEqual(expected, resourceManagerWrapper.ResourceSets.Select(x => x.Culture.TwoLetterISOLanguageName));
        }
    }
}
