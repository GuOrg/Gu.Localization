namespace Gu.Localization.Tests.Sandbox
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
                CultureInfo.GetCultureInfo(name);

                name = culture.TwoLetterISOLanguageName;
                Assert.IsTrue(Culture.Exists(name));
                CultureInfo.GetCultureInfo(name);
            }
        }
    }
}