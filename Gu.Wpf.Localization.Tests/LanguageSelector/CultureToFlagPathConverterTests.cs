namespace Gu.Wpf.Localization.Tests.LanguageSelector
{
    using System.Globalization;
    using NUnit.Framework;

    public class CultureToFlagPathConverterTests
    {
        //[TestCase("se", "")]
        [TestCase("sv-SE", "pack://application:,,,/Gu.Wpf.Localization;component/Flags/se.png")]
        [TestCase("fi-FI", "pack://application:,,,/Gu.Wpf.Localization;component/Flags/fi.png")]
        //[TestCase("FI", "")]
        //[TestCase("en-GB", "")]
        public void TryGetCultureFromRegion(string cultureName, string path)
        {
            Assert.AreEqual(true, CultureToFlagPathConverter.TryGetFlagPath(CultureInfo.GetCultureInfo(cultureName), out var actual));
            Assert.AreEqual(path, actual);
        }
    }
}
