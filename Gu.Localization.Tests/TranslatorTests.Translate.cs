namespace Gu.Localization.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Gu.Localization;

    using NUnit.Framework;

    public static partial class TranslatorTests
    {
        public static class Translate
        {
            [TestCase("en", "English")]
            [TestCase("sv", "Svenska")]
            [TestCase(null, "So neutral")]
            public static void TranslateResourceManagerAndNameHappyPath(string cultureName, string expected)
            {
                var culture = cultureName != null
                                  ? CultureInfo.GetCultureInfo(cultureName)
                                  : CultureInfo.InvariantCulture;
                Translator.Culture = culture;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
                Assert.AreEqual(expected, actual);

                Translator.Culture = null;
                actual = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), culture);
                Assert.AreEqual(expected, actual);
            }

            [TestCaseSource(typeof(TranslationSource))]
            public static void TranslateWithGlobalCulture(TranslationSource.Row row)
            {
                Translator.Culture = row.Culture;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, row.Key);
                Assert.AreEqual(row.ExpectedTranslation, actual);

                foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                                                  .OfType<ErrorHandling>())
                {
                    actual = Translator.Translate(Properties.Resources.ResourceManager, row.Key, errorHandling);
                    Assert.AreEqual(row.ExpectedTranslation, actual);
                }

                foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                                  .OfType<ErrorHandling>())
                {
                    Translator.ErrorHandling = errorHandling;
                    actual = Translator.Translate(Properties.Resources.ResourceManager, row.Key);
                    Assert.AreEqual(row.ExpectedTranslation, actual);
                }
            }

            [TestCaseSource(typeof(TranslationSource))]
            public static void TranslateWithExplicitCulture(TranslationSource.Row row)
            {
                Translator.Culture = null;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, row.Key, row.Culture);
                Assert.AreEqual(row.ExpectedTranslation, actual);

                foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                                                  .OfType<ErrorHandling>())
                {
                    actual = Translator.Translate(Properties.Resources.ResourceManager, row.Key, row.Culture, errorHandling);
                    Assert.AreEqual(row.ExpectedTranslation, actual);
                }

                foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                                  .OfType<ErrorHandling>())
                {
                    Translator.ErrorHandling = errorHandling;
                    actual = Translator.Translate(Properties.Resources.ResourceManager, row.Key, row.Culture);
                    Assert.AreEqual(row.ExpectedTranslation, actual);
                }
            }

            [TestCaseSource(typeof(TranslationErrorsSource))]
            public static void WithGlobalErrorHandling(TranslationErrorsSource.ErrorData data)
            {
                if (!Translator.ContainsCulture(data.Culture))
                {
                    Assert.Pass("nop");
                }

                Translator.Culture = data.Culture;
                Translator.ErrorHandling = data.ErrorHandling;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, data.Key!);
                Assert.AreEqual(data.ExpectedTranslation, actual);
            }

            [TestCaseSource(typeof(TranslationErrorsSource))]
            public static void WithExplicitErrorHandling(TranslationErrorsSource.ErrorData data)
            {
                if (!Translator.ContainsCulture(data.Culture))
                {
                    Assert.Pass("nop");
                }

                Translator.Culture = data.Culture;
                Translator.ErrorHandling = ErrorHandling.Throw;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, data.Key!, data.ErrorHandling);
                Assert.AreEqual(data.ExpectedTranslation, actual);
            }

            [TestCaseSource(typeof(TranslationErrorsSource))]
            public static void WithExplicitErrorHandlingAndCulture(TranslationErrorsSource.ErrorData data)
            {
                Translator.Culture = null;
                Translator.ErrorHandling = ErrorHandling.Throw;
                var actual = Translator.Translate(Properties.Resources.ResourceManager, data.Key!, data.Culture, data.ErrorHandling);
                Assert.AreEqual(data.ExpectedTranslation, actual);
            }

            [TestCaseSource(typeof(TranslationThrowSource))]
            public static void ThrowsWithGlobalErrorHandling(TranslationThrowSource.ErrorData data)
            {
                if (!Translator.ContainsCulture(data.Culture!))
                {
                    Assert.Pass("nop");
                }

                Translator.Culture = data.Culture;
                Translator.ErrorHandling = data.ErrorHandling;
                var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, data.Key!));
                Assert.AreEqual(data.ExpectedMessage, actual.Message);

                actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, data.Key!, ErrorHandling.Inherit));
                Assert.AreEqual(data.ExpectedMessage, actual.Message);
            }

            [TestCaseSource(typeof(TranslationThrowSource))]
            public static void ThrowsWithExplicitErrorHandling(TranslationThrowSource.ErrorData data)
            {
                if (!Translator.ContainsCulture(data.Culture!))
                {
                    Assert.Pass("nop");
                }

                Translator.Culture = data.Culture;
                Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
                var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, data.Key!, data.ErrorHandling));
#if DEBUG
                Console.Write(actual.Message);
#endif

                Assert.AreEqual(data.ExpectedMessage, actual.Message);
            }

            [TestCaseSource(typeof(TranslationThrowSource))]
            public static void ThrowsWithExplicitErrorHandlingAndCulture(TranslationThrowSource.ErrorData data)
            {
                Translator.Culture = null;
                Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
                var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator.Translate(Properties.Resources.ResourceManager, data.Key!, data.Culture!, data.ErrorHandling));
#if DEBUG
                Console.Write(actual.Message);
#endif
                Assert.AreEqual(data.ExpectedMessage, actual.Message);
            }
        }
    }
}
