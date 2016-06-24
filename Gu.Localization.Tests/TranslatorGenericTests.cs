namespace Gu.Localization.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;

    using NUnit.Framework;

    public partial class TranslatorGenericTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelpers.ClearTranslationCache();
        }

        [TestCaseSource(typeof(TranslationSource))]
        public void TranslateWithGlobalCulture(TranslationSource.Row row)
        {
            Translator.Culture = row.Culture;
            var actual = Translator<Properties.Resources>.Translate(row.Key);
            Assert.AreEqual(row.ExpectedTranslation, actual);

            foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                                              .OfType<ErrorHandling>())
            {
                actual = Translator<Properties.Resources>.Translate(row.Key, errorHandling);
                Assert.AreEqual(row.ExpectedTranslation, actual);
            }

            foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                              .OfType<ErrorHandling>())
            {
                Translator.ErrorHandling = errorHandling;
                actual = Translator<Properties.Resources>.Translate(row.Key);
                Assert.AreEqual(row.ExpectedTranslation, actual);
            }
        }

        [TestCaseSource(typeof(TranslationSource))]
        public void TranslateWithExplicitCulture(TranslationSource.Row row)
        {
            Translator.Culture = null;
            var actual = Translator<Properties.Resources>.Translate(row.Key, row.Culture);
            Assert.AreEqual(row.ExpectedTranslation, actual);

            foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                                              .OfType<ErrorHandling>())
            {
                actual = Translator<Properties.Resources>.Translate(row.Key, row.Culture, errorHandling);
                Assert.AreEqual(row.ExpectedTranslation, actual);
            }

            foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling))
                              .OfType<ErrorHandling>())
            {
                Translator.ErrorHandling = errorHandling;
                actual = Translator<Properties.Resources>.Translate(row.Key, row.Culture);
                Assert.AreEqual(row.ExpectedTranslation, actual);
            }
        }

        [Test]
        public void CreateTranslation()
        {
            Translator.Culture = CultureInfo.GetCultureInfo("en");
            var translation = Translator<Properties.Resources>.GetOrCreateTranslation(nameof(Properties.Resources.AllLanguages));

            Assert.AreEqual("English", translation.Translated);
            Translator.Culture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual("Svenska", translation.Translated);
        }

        [TestCaseSource(typeof(TranslationErrorsSource))]
        public void WithGlobalErrorhandling(TranslationErrorsSource.ErrorData data)
        {
            if (!Translator.ContainsCulture(data.Culture))
            {
                Assert.Pass("nop");
            }

            Translator.Culture = data.Culture;
            Translator.ErrorHandling = data.ErrorHandling;
            var actual = Translator<Properties.Resources>.Translate(data.Key);
            Assert.AreEqual(data.ExpectedTranslation, actual);
        }

        [TestCaseSource(typeof(TranslationErrorsSource))]
        public void WithExplicitErrorhandling(TranslationErrorsSource.ErrorData data)
        {
            if (!Translator.ContainsCulture(data.Culture))
            {
                Assert.Pass("nop");
            }

            Translator.Culture = data.Culture;
            Translator.ErrorHandling = ErrorHandling.Throw;
            var actual = Translator<Properties.Resources>.Translate(data.Key, data.ErrorHandling);
            Assert.AreEqual(data.ExpectedTranslation, actual);
        }

        [TestCaseSource(typeof(TranslationErrorsSource))]
        public void WithExplicitErrorhandlingAndCulture(TranslationErrorsSource.ErrorData data)
        {
            Translator.Culture = null;
            Translator.ErrorHandling = ErrorHandling.Throw;
            var actual = Translator<Properties.Resources>.Translate(data.Key, data.Culture, data.ErrorHandling);
            Assert.AreEqual(data.ExpectedTranslation, actual);
        }

        [TestCaseSource(typeof(TranslationThrowSource))]
        public void ThrowsWithGlobalErrorhandling(TranslationThrowSource.ErrorData data)
        {
            if (!Translator.ContainsCulture(data.Culture))
            {
                Assert.Pass("nop");
            }

            Translator.Culture = data.Culture;
            Translator.ErrorHandling = data.ErrorHandling;
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(data.Key));
            Assert.AreEqual(data.ExpectedMessage, actual.Message);

            actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(data.Key, ErrorHandling.Inherit));
            Assert.AreEqual(data.ExpectedMessage, actual.Message);
        }

        [TestCaseSource(typeof(TranslationThrowSource))]
        public void ThrowsWithExplicitErrorhandling(TranslationThrowSource.ErrorData data)
        {
            if (!Translator.ContainsCulture(data.Culture))
            {
                Assert.Pass("nop");
            }

            Translator.Culture = data.Culture;
            Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(data.Key, data.ErrorHandling));
            Assert.AreEqual(data.ExpectedMessage, actual.Message);
        }

        [TestCaseSource(typeof(TranslationThrowSource))]
        public void ThrowsWithExplicitErrorhandlingAndCulture(TranslationThrowSource.ErrorData data)
        {
            Translator.Culture = null;
            Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(data.Key, data.Culture, data.ErrorHandling));
            Assert.AreEqual(data.ExpectedMessage, actual.Message);
        }
    }
}