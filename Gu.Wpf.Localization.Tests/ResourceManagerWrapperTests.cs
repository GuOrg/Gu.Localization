namespace Gu.Wpf.Localization.Tests
{
    using System.Linq;
    using System.Threading;

    using NUnit.Framework;

    public class ResourceManagerWrapperTests
    {
        [Test]
        public void Languages()
        {
            var resourceManagerWrapper = new ResourceManagerWrapper(Properties.Resources.ResourceManager);
            var expected = new[]{"en","sv"};
            CollectionAssert.AreEqual(expected,resourceManagerWrapper.ResourceSets.Select(x=>x.Culture.TwoLetterISOLanguageName));            
        }
    }
}
