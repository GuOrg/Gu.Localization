namespace Gu.Localization.Tests.Internals
{
    using System.Collections.Generic;
    using System.Globalization;
    using NUnit.Framework;

    public class CultureTests
    {
        private static readonly IReadOnlyList<CultureInfo> AllCultures = Culture.AllCultures;

        [TestCaseSource(nameof(AllCultures))]
        public void TryGetByName(CultureInfo cultureInfo)
        {
            Assert.AreEqual(true, Culture.TryGet(cultureInfo.Name, out var match));
            Assert.AreEqual(cultureInfo, match);
        }

        [TestCaseSource(nameof(AllCultures))]
        public void TryGetByTwoLetterISOLanguageName(CultureInfo cultureInfo)
        {
            Assert.AreEqual(true, Culture.TryGet(cultureInfo.TwoLetterISOLanguageName, out var match));
            Assert.AreEqual(cultureInfo.TwoLetterISOLanguageName, match.TwoLetterISOLanguageName);
        }

        [TestCase("sv", "SE")]
        [TestCase("sv-SE", "SE")]
        [TestCase("sv-FI", "FI")]
        [TestCase("en", "US")]
        [TestCase("en-US", "US")]
        [TestCase("en-GB", "GB")]
        public void TryGetRegion(string cultureName, string regionName)
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            Assert.AreEqual(true, Culture.TryGetRegion(culture, out var region));
            Assert.AreEqual(regionName, region.TwoLetterISORegionName);
        }
    }
}
