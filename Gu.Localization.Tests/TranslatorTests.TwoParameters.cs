namespace Gu.Localization.Tests
{
    using System;
    using System.Globalization;

    using NUnit.Framework;

    public static partial class TranslatorTests
    {
        public static class TwoParameters
        {
            [TestCase("en", 1, 2.0, "English first: 1, second 2.00")]
            [TestCase("sv", 1, 2.0, "första: 1, andra: 2,00")]
            [TestCase(null, 1, 2.0, "Neutral first: 1, second 2.00")]
            public static void HappyPath(string cultureName, object arg0, object arg1, string expected)
            {
                var culture = cultureName != null
                                         ? CultureInfo.GetCultureInfo(cultureName)
                                         : CultureInfo.InvariantCulture;
                var key = nameof(Properties.Resources.ValidFormat__0__1__);

                Translator.Culture = culture;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, key, arg0, arg1);
                Assert.AreEqual(expected, actual);

                Translator.Culture = null;
                actual = Translator.Translate(Properties.Resources.ResourceManager, key, culture, arg0, arg1);
                Assert.AreEqual(expected, actual);
            }

            [TestCase("en", 1, 2, "Invalid format string: \"Value: {0} {2}\".")]
            [TestCase("sv", 1, 2, "Invalid format string: \"Värde: \" for the two arguments: 1, 2.")]
            public static void Throws(string cultureName, object arg0, object arg1, string expected)
            {
                var culture = cultureName != null
                                         ? CultureInfo.GetCultureInfo(cultureName)
                                         : CultureInfo.InvariantCulture;
                var key = nameof(Properties.Resources.InvalidFormat__0__);

                Translator.Culture = culture;
                var actual = Assert.Throws<FormatException>(() => Translator.Translate(Properties.Resources.ResourceManager, key, arg0, arg1));
                Assert.AreEqual(expected, actual.Message);

                Translator.Culture = null;
                actual = Assert.Throws<FormatException>(() => Translator.Translate(Properties.Resources.ResourceManager, key, culture, arg0, arg1));
                Assert.AreEqual(expected, actual.Message);
            }

            [TestCase("en", 1, 2, "{\"Value: {0} {2}\" : 1, 2}")]
            [TestCase("sv", 1, 2, "{\"Värde: \" : 1, 2}")]
            public static void ReturnsInfo(string cultureName, object arg0, object arg1, string expected)
            {
                var culture = cultureName != null
                                         ? CultureInfo.GetCultureInfo(cultureName)
                                         : CultureInfo.InvariantCulture;
                var key = nameof(Properties.Resources.InvalidFormat__0__);

                Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
                Translator.Culture = culture;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, key, arg0, arg1);
                Assert.AreEqual(expected, actual);

                Translator.ErrorHandling = ErrorHandling.Throw;
                Translator.Culture = null;
                actual = Translator.Translate(Properties.Resources.ResourceManager, key, culture, arg0, arg1, ErrorHandling.ReturnErrorInfo);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
