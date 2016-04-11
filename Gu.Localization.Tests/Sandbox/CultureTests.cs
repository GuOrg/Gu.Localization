namespace Gu.Localization.Tests.Sandbox
{
    using System.Globalization;
    using System.Linq;
    using NUnit.Framework;

    public class CultureTests
    {
        [Test]
        public void RoundTrip()
        {
            var cultures =  CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .ToArray();
            foreach (var culture in cultures)
            {
                Assert.IsTrue(Culture.Exists(culture.Name));
                Assert.IsTrue(Culture.Exists(culture.TwoLetterISOLanguageName));
                CultureInfo.GetCultureInfo(culture.Name);
                CultureInfo.GetCultureInfo(culture.TwoLetterISOLanguageName);
            }
        }
    }
}