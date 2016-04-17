namespace Gu.Localization.Tests.Internals
{
    using System.Globalization;

    using NUnit.Framework;

    public class ResourceManagerExtTests
    {
        [TestCase(null, true)]
        [TestCase("sv", true)]
        [TestCase("it", false)]
        public void HasCulture(string cultureName, bool expected)
        {
            var culture = cultureName == null
                                 ? CultureInfo.InvariantCulture
                                 : CultureInfo.GetCultureInfo(cultureName);

            Assert.AreEqual(expected, Properties.Resources.ResourceManager.HasCulture(culture));
            Assert.IsNull(Properties.Resources.ResourceManager.GetResourceSet(culture, false, false));
        }

        [TestCase(nameof(Properties.Resources.AllLanguages), "sv", true)]
        [TestCase(nameof(Properties.Resources.EnglishOnly), "sv", false)]
        [TestCase(nameof(Properties.Resources.EnglishOnly), "en", true)]
        public void HasKey(string key, string cultureName, bool expected)
        {
            var culture = cultureName == null
                                 ? CultureInfo.InvariantCulture
                                 : CultureInfo.GetCultureInfo(cultureName);

            Properties.Resources.ResourceManager.GetString(key, culture); // warmup

            Assert.AreEqual(expected, Properties.Resources.ResourceManager.HasKey(key, culture, false));
            Assert.IsNull(Properties.Resources.ResourceManager.GetResourceSet(culture, false, false));
        }

        [Test]
        public void HasCultureThenHasKey()
        {
            var italian = CultureInfo.GetCultureInfo("it");
            var key = nameof(Properties.Resources.AllLanguages);

            Assert.AreEqual(false, Properties.Resources.ResourceManager.HasCulture(italian));
            Properties.Resources.ResourceManager.GetString(key, italian); // warmup

            Assert.AreEqual(false, Properties.Resources.ResourceManager.HasKey(key, italian, false));
        }

        [Test]
        public void HasKeyThenHasCulture()
        {
            var italian = CultureInfo.GetCultureInfo("it");
            var key = nameof(Properties.Resources.AllLanguages);

            Assert.AreEqual(false, Properties.Resources.ResourceManager.HasCulture(italian));
            Properties.Resources.ResourceManager.GetString(key, italian); // warmup

            Assert.AreEqual(false, Properties.Resources.ResourceManager.HasKey(key, italian, false));
        }
    }
}