namespace Gu.Localization.Tests
{
    using System;
    using System.Globalization;

    using NUnit.Framework;

    public partial class TranslatorGenericTests
    {
        public class OneParameter
        {
            [OneTimeSetUp]
            public void SetUp()
            {
                TestHelpers.ClearTranslationCache();
            }

            [TestCase("en", 1, "Value: 1")]
            [TestCase("sv", 1, "Värde: 1")]
            [TestCase(null, 1, "Neutral: 1")]
            public void HappyPath(string cultureName, object arg, string expected)
            {
                var culture = cultureName != null
                                  ? CultureInfo.GetCultureInfo(cultureName)
                                  : CultureInfo.InvariantCulture;
                var key = nameof(Properties.Resources.ValidFormat__0__);

                Translator.Culture = culture;
                var actual = Translator<Properties.Resources>.Translate(key, arg);
                Assert.AreEqual(expected, actual);

                Translator.Culture = null;
                actual = Translator<Properties.Resources>.Translate(key, culture, arg);
                Assert.AreEqual(expected, actual);
            }

            [TestCase("en", 1, "Invalid format string: \"Value: {0} {2}\".")]
            [TestCase("sv", 1, "Invalid format string: \"Värde: \" for the single argument: 1.")]
            public void Throws(string cultureName, object arg, string expected)
            {
                var culture = cultureName != null
                                  ? CultureInfo.GetCultureInfo(cultureName)
                                  : CultureInfo.InvariantCulture;
                var key = nameof(Properties.Resources.InvalidFormat__0__);

                Translator.Culture = culture;
                var actual = Assert.Throws<FormatException>(() => Translator<Properties.Resources>.Translate(key, arg));
                Assert.AreEqual(expected, actual.Message);

                Translator.Culture = null;
                actual = Assert.Throws<FormatException>(() => Translator<Properties.Resources>.Translate(key, culture, arg));
                Assert.AreEqual(expected, actual.Message);
            }

            [TestCase("en", 1, "{\"Value: {0} {2}\" : 1}")]
            [TestCase("sv", 1, "{\"Värde: \" : 1}")]
            public void ReturnsInfo(string cultureName, object arg, string expected)
            {
                var culture = cultureName != null
                                  ? CultureInfo.GetCultureInfo(cultureName)
                                  : CultureInfo.InvariantCulture;
                var key = nameof(Properties.Resources.InvalidFormat__0__);

                Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
                Translator.Culture = culture;
                var actual = Translator<Properties.Resources>.Translate(key, arg);
                Assert.AreEqual(expected, actual);

                Translator.ErrorHandling = ErrorHandling.Throw;
                Translator.Culture = null;
                actual = Translator<Properties.Resources>.Translate(key, culture, arg, ErrorHandling.ReturnErrorInfo);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}