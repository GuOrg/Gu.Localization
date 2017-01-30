namespace Gu.Localization.Tests.Internals
{
    using System.Globalization;

    using NUnit.Framework;

    public class ResourceManagerExtTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Properties.Resources.ResourceManager.ReleaseAllResources();
        }

        [Test]
        public void HasNeutralCulture()
        {
            var resourceManager = Properties.Resources.ResourceManager;
            Assert.AreEqual(true, resourceManager.HasCulture(null));
            Assert.AreEqual(true, resourceManager.HasCulture(CultureInfo.InvariantCulture));
        }

        [TestCase(null, true)]
        [TestCase("sv", true)]
        [TestCase("en", true)]
        [TestCase("it", false)]
        public void HasCulture(string cultureName, bool expected)
        {
            var culture = cultureName == null
                                 ? CultureInfo.InvariantCulture
                                 : CultureInfo.GetCultureInfo(cultureName);

            var resourceManager = Properties.Resources.ResourceManager;
            Assert.AreEqual(expected, resourceManager.HasCulture(culture));
            Assert.IsNull(resourceManager.GetResourceSet(culture, false, false));
        }

        [TestCase(nameof(Properties.Resources.AllLanguages), null, true)]
        [TestCase(nameof(Properties.Resources.AllLanguages), "sv", true)]
        [TestCase(nameof(Properties.Resources.AllLanguages), "en", true)]
        [TestCase(nameof(Properties.Resources.EnglishOnly), "sv", false)]
        [TestCase(nameof(Properties.Resources.EnglishOnly), "en", true)]
        public void HasKey(string key, string cultureName, bool expected)
        {
            var culture = cultureName == null
                                 ? CultureInfo.InvariantCulture
                                 : CultureInfo.GetCultureInfo(cultureName);

            var resourceManager = Properties.Resources.ResourceManager;
            Assert.AreEqual(expected, resourceManager.HasKey(key, culture));
            Assert.IsNull(resourceManager.GetResourceSet(culture, false, false));
        }

        [TestCase(nameof(Properties.Resources.AllLanguages),  true)]
        [TestCase(nameof(Properties.Resources.EnglishOnly), true)]
        public void HasKeyInvariantAndNullCulture(string key, bool expected)
        {
            var resourceManager = Properties.Resources.ResourceManager;
            Assert.AreEqual(expected, resourceManager.HasKey(key, null));
            Assert.AreEqual(expected, resourceManager.HasKey(key, CultureInfo.InvariantCulture));
        }

        [Test]
        public void HasCultureThenHasKey()
        {
            var italian = CultureInfo.GetCultureInfo("it");
            var key = nameof(Properties.Resources.AllLanguages);
            var resourceManager = Properties.Resources.ResourceManager;
            Assert.AreEqual(false, resourceManager.HasCulture(italian));
            Assert.AreEqual(false, resourceManager.HasKey(key, italian));
            Assert.IsNull(resourceManager.GetResourceSet(italian, false, false));
        }

        [Test]
        public void HasKeyThenHasCulture()
        {
            var italian = CultureInfo.GetCultureInfo("it");
            var key = nameof(Properties.Resources.AllLanguages);

            var resourceManager = Properties.Resources.ResourceManager;
            Assert.AreEqual(false, resourceManager.HasCulture(italian));
            Assert.AreEqual(false, resourceManager.HasKey(key, italian));
            Assert.IsNull(resourceManager.GetResourceSet(italian, false, false));
        }
    }
}