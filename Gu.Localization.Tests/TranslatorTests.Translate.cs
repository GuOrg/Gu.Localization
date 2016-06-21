namespace Gu.Localization.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Gu.Localization;

    using NUnit.Framework;

    public partial class TranslatorTests
    {
        public class Translate
        {
            [TestCase("en", "English")]
            [TestCase("sv", "Svenska")]
            [TestCase(null, "So neutral")]
            public void TranslateResourceManagerAndNameHappyPath(string cultureName, string expected)
            {
                var culture = cultureName != null
                                  ? CultureInfo.GetCultureInfo(cultureName)
                                  : CultureInfo.InvariantCulture;
                Translator.CurrentCulture = culture;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
                Assert.AreEqual(expected, actual);

                Translator.CurrentCulture = null;
                actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), culture);
                Assert.AreEqual(expected, actual);
            }

            [TestCase(nameof(Properties.Resources.NeutralOnly), null, "So neutral")]
            [TestCase(nameof(Properties.Resources.AllLanguages), "en", "English")]
            [TestCase(nameof(Properties.Resources.AllLanguages), "sv", "Svenska")]
            [TestCase(nameof(Properties.Resources.AllLanguages), null, "So neutral")]
            public void HappyPath(string key, string culture, string expected)
            {
                Translator.CurrentCulture = culture == null
                                                ? CultureInfo.InvariantCulture
                                                : CultureInfo.GetCultureInfo(culture);
                var actual = Translator.Translate(Properties.Resources.ResourceManager, key);
                Assert.AreEqual(expected, actual);

                foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                                                  .OfType<ErrorHandling>())
                {
                    actual = Translator.Translate(Properties.Resources.ResourceManager, key, errorHandling);
                    Assert.AreEqual(expected, actual);
                }

                foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                                                  .OfType<ErrorHandling>())
                {
                    Translator.ErrorHandling = errorHandling;
                    actual = Translator.Translate(Properties.Resources.ResourceManager, key);
                    Assert.AreEqual(expected, actual);
                }
            }

            [TestCaseSource(typeof(TranslationErrorsSource))]
            public void WithGlobalErrorhandling(TranslationErrorsSource.ErrorData data)
            {
                if (!Translator.ContainsCulture(data.Culture))
                {
                    Assert.Pass("nop");
                }

                Translator.CurrentCulture = data.Culture;
                Translator.ErrorHandling = data.ErrorHandling;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, data.Key);
                Assert.AreEqual(data.ExpectedTranslation, actual);
            }

            [TestCaseSource(typeof(TranslationErrorsSource))]
            public void WithExplicitErrorhandling(TranslationErrorsSource.ErrorData data)
            {
                if (!Translator.ContainsCulture(data.Culture))
                {
                    Assert.Pass("nop");
                }

                Translator.CurrentCulture = data.Culture;
                Translator.ErrorHandling = ErrorHandling.Throw;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, data.Key, data.ErrorHandling);
                Assert.AreEqual(data.ExpectedTranslation, actual);
            }

            [TestCaseSource(typeof(TranslationErrorsSource))]
            public void WithExplicitErrorhandlingAndCulture(TranslationErrorsSource.ErrorData data)
            {
                Translator.CurrentCulture = null;
                Translator.ErrorHandling = ErrorHandling.Throw;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, data.Key, data.Culture, data.ErrorHandling);
                Assert.AreEqual(data.ExpectedTranslation, actual);
            }

            [TestCaseSource(typeof(TranslationThrowSource))]
            public void ThrowsWithGlobalErrorhandling(TranslationThrowSource.ErrorData data)
            {
                if (!Translator.ContainsCulture(data.Culture))
                {
                    Assert.Pass("nop");
                }

                Translator.CurrentCulture = data.Culture;
                Translator.ErrorHandling = data.ErrorHandling;
                var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, data.Key));
                Assert.AreEqual(data.ExpectedMessage, actual.Message);

                actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, data.Key, ErrorHandling.Default));
                Assert.AreEqual(data.ExpectedMessage, actual.Message);
            }

            [TestCaseSource(typeof(TranslationThrowSource))]
            public void ThrowsWithExplicitErrorhandling(TranslationThrowSource.ErrorData data)
            {
                if (!Translator.ContainsCulture(data.Culture))
                {
                    Assert.Pass("nop");
                }

                Translator.CurrentCulture = data.Culture;
                Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
                var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, data.Key, data.ErrorHandling));
                Assert.AreEqual(data.ExpectedMessage, actual.Message);
            }

            [TestCaseSource(typeof(TranslationThrowSource))]
            public void ThrowsWithExplicitErrorhandlingAndCulture(TranslationThrowSource.ErrorData data)
            {
                Translator.CurrentCulture = null;
                Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
                var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, data.Key, data.Culture, data.ErrorHandling));
                Assert.AreEqual(data.ExpectedMessage, actual.Message);
            }
        }
    }
}