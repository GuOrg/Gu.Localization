namespace Gu.Localization.Tests
{
    using System.Globalization;

    using NUnit.Framework;

    public partial class TranslatorTests
    {
        public class Parameters
        {
            [TestCase("en", 1, "Value: 1")]
            [TestCase("sv", 1, "Värde: 1")]
            [TestCase(null, 1, "Neutral: 1")]
            public void TranslateOneParameterThrow(string cultureName, object arg, string expected)
            {
                Assert.Inconclusive();
                var culture = cultureName != null
                                         ? CultureInfo.GetCultureInfo(cultureName)
                                         : CultureInfo.InvariantCulture;
                Translator.CurrentCulture = culture;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Value___0_), arg);
                Assert.AreEqual(expected, actual);

                Translator.CurrentCulture = CultureInfo.GetCultureInfo("it");
                actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Value___0_), culture, arg);
                Assert.AreEqual(expected, actual);
            }

            [Test]
            public void TranslateOneParameterReturnInfo()
            {
                Assert.Inconclusive();
            }
        }
    }
}
