namespace Gu.Localization.Tests.Internals
{
    using System.Globalization;
    using System.Linq;

    using NUnit.Framework;

    public class CultureTests
    {
        private static readonly IReadOnlyList<CultureInfo> AllCultures = CultureInfo
                                                                         .GetCultures(CultureTypes.AllCultures)
                                                                         .Where(x => !string.IsNullOrEmpty(x.Name))
                                                                         .ToArray();

        [TestCaseSource(nameof(AllCultures))]
        public void ExistsByName(CultureInfo cultureInfo)
        {
            Assert.AreEqual(true, Culture.Exists(cultureInfo.Name));
        }

        [TestCaseSource(nameof(AllCultures))]
        public void ExistsByTwoLetterISOLanguageName(CultureInfo cultureInfo)
        {
            Assert.AreEqual(true, Culture.Exists(cultureInfo.TwoLetterISOLanguageName));
        }

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
            Assert.AreEqual(true, Culture.TryGetRegion(cultureName, out var region));
            Assert.AreEqual(regionName, region.TwoLetterISORegionName);
        }
    }
}
