namespace Gu.Wpf.Localization.Tests.LanguageSelector
{
    using System.Globalization;
    using NUnit.Framework;

    public class CultureToFlagPathConverterTests
    {
        [TestCase("sv", "pack://application:,,,/Gu.Wpf.Localization;component/Flags/se.png")]
        [TestCase("sv-SE", "pack://application:,,,/Gu.Wpf.Localization;component/Flags/se.png")]
        [TestCase("fi-FI", "pack://application:,,,/Gu.Wpf.Localization;component/Flags/fi.png")]
        [TestCase("FI", "pack://application:,,,/Gu.Wpf.Localization;component/Flags/fi.png")]
        [TestCase("en", "pack://application:,,,/Gu.Wpf.Localization;component/Flags/us.png")]
        [TestCase("en-US", "pack://application:,,,/Gu.Wpf.Localization;component/Flags/us.png")]
        [TestCase("en-GB", "pack://application:,,,/Gu.Wpf.Localization;component/Flags/gb.png")]
        public void TryGetFlagPath(string cultureName, string path)
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            Assert.AreEqual(true, CultureToFlagPathConverter.TryGetFlagPath(culture, out var actual));
            Assert.AreEqual(path, actual);
        }
    }
}
