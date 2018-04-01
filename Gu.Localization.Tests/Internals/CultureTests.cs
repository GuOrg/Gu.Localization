namespace Gu.Localization.Tests.Internals
{
    using System.Globalization;
    using System.Linq;

    using NUnit.Framework;

    public class CultureTests
    {
        [Test]
        public void RoundTripAll()
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                                      .Where(x => !string.IsNullOrEmpty(x.Name))
                                      .ToArray();
            foreach (var culture in cultures)
            {
                var name = culture.Name;
                Assert.IsTrue(Culture.Exists(name));
                Assert.AreEqual(name, CultureInfo.GetCultureInfo(name).Name);

                name = culture.TwoLetterISOLanguageName;
                Assert.IsTrue(Culture.Exists(name));
                Assert.AreEqual(name, CultureInfo.GetCultureInfo(name).Name);
            }
        }

        [TestCase("sv", "SE")]
        [TestCase("sv-SE", "SE")]
        [TestCase("sv-FI", "FI")]
        public void TryGetRegion(string cultureName, string regionName)
        {
            Assert.AreEqual(true, Culture.TryGetRegion(cultureName, out var region));
            Assert.AreEqual(regionName, region.TwoLetterISORegionName);
        }
    }
}
