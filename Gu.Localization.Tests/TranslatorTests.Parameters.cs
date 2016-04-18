namespace Gu.Localization.Tests
{
    using System;
    using System.Globalization;

    using NUnit.Framework;

    public partial class TranslatorTests
    {
        public class Parameters
        {
            [TestCase("en", 1, "Value: 1")]
            [TestCase("sv", 1, "Värde: 1")]
            [TestCase(null, 1, "Neutral: 1")]
            public void TranslateOneParameterHappyPath(string cultureName, object arg, string expected)
            {
                var culture = cultureName != null
                                         ? CultureInfo.GetCultureInfo(cultureName)
                                         : CultureInfo.InvariantCulture;
                Translator.CurrentCulture = culture;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.ValidFormat__0__), arg);
                Assert.AreEqual(expected, actual);

                Translator.CurrentCulture = CultureInfo.GetCultureInfo("it");
                actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.ValidFormat__0__), culture, arg);
                Assert.AreEqual(expected, actual);
            }

            [TestCase("en", 1, "Invalid format string: \"Value: {0} {1}\" for the single argument: 1.")]
            [TestCase("sv", 1, "Invalid format string: \"Värde: \" for the single argument: 1.")]
            public void TranslateOneParameterThrow(string cultureName, object arg, string expected)
            {
                var culture = cultureName != null
                                         ? CultureInfo.GetCultureInfo(cultureName)
                                         : CultureInfo.InvariantCulture;
                Translator.CurrentCulture = culture;
                var actual = Assert.Throws<FormatException>(() => Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.InvalidFormat__0__), arg));
                Assert.AreEqual(expected, actual.Message);

                Translator.CurrentCulture = CultureInfo.GetCultureInfo("it");
                actual = Assert.Throws<FormatException>(() => Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.InvalidFormat__0__), culture, arg));
                Assert.AreEqual(expected, actual.Message);
            }

            [TestCase("en", 1, "{\"Value: {0} {1}\" : 1}")]
            [TestCase("sv", 1, "{\"Värde: \" : 1}")]
            public void TranslateOneParameterReturnInfo(string cultureName, object arg, string expected)
            {
                var culture = cultureName != null
                                         ? CultureInfo.GetCultureInfo(cultureName)
                                         : CultureInfo.InvariantCulture;
                Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
                Translator.CurrentCulture = culture;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.InvalidFormat__0__), arg);
                Assert.AreEqual(expected, actual);

                Translator.ErrorHandling = ErrorHandling.Throw;
                Translator.CurrentCulture = CultureInfo.GetCultureInfo("it");
                actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.InvalidFormat__0__), culture, arg, ErrorHandling.ReturnErrorInfo);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
