////namespace Gu.Localization.Tests
////{
////    using System;
////    using System.Globalization;

////    using NUnit.Framework;

////    public partial class TranslatorTests
////    {
////        public class Params
////        {
////            [TestCase("en", 1, 2.0, 3, "English first: 1, second 2.00, third: 3")]
////            [TestCase("sv", 1, 2.0, 3, "första: 1, andra: 2,00, tredje: 3")]
////            [TestCase(null, 1, 2.0, 3, "Neutral first: 1, second 2.00, third: 3")]
////            public void HappyPath(string cultureName, object arg0, object arg1, object arg2, string expected)
////            {
////                var culture = cultureName != null
////                                         ? CultureInfo.GetCultureInfo(cultureName)
////                                         : CultureInfo.InvariantCulture;
////                var key = nameof(Properties.Resources.ValidFormat__0__1__2__);

////                Translator.Culture = culture;
////                var actual = Translator.Translate(Properties.Resources.ResourceManager, key, arg0, arg1, arg2);
////                Assert.AreEqual(expected, actual);

////                Translator.Culture = null;
////                actual = Translator.Translate(Properties.Resources.ResourceManager, key, culture, arg0, arg1, arg2);
////                Assert.AreEqual(expected, actual);
////            }

////            [TestCase("en", 1, 2, 3, "Invalid format string: \"Value: {0} {2}\".")]
////            [TestCase("sv", 1, 2,3, "Invalid format string: \"Värde: \" for the two arguments: 1, 2, 3.")]
////            public void Throws(string cultureName, object arg0, object arg1, object arg2, string expected)
////            {
////                var culture = cultureName != null
////                                         ? CultureInfo.GetCultureInfo(cultureName)
////                                         : CultureInfo.InvariantCulture;
////                var key = nameof(Properties.Resources.InvalidFormat__0__);

////                Translator.Culture = culture;
////                var actual = Assert.Throws<FormatException>(() => Translator.Translate(Properties.Resources.ResourceManager, key, arg0, arg1, arg2));
////                Assert.AreEqual(expected, actual.Message);

////                Translator.Culture = null;
////                actual = Assert.Throws<FormatException>(() => Translator.Translate(Properties.Resources.ResourceManager, key, culture, arg0, arg1, arg2));
////                Assert.AreEqual(expected, actual.Message);
////            }

////            [TestCase("en", 1, 2, 3, "{\"Value: {0} {2}\" : 1, 2, 3}")]
////            [TestCase("sv", 1, 2, 3, "{\"Värde: \" : 1, 2, 3}")]
////            public void ReturnsInfo(string cultureName, object arg0, object arg1, object arg2, string expected)
////            {
////                var culture = cultureName != null
////                                         ? CultureInfo.GetCultureInfo(cultureName)
////                                         : CultureInfo.InvariantCulture;
////                var key = nameof(Properties.Resources.InvalidFormat__0__);

////                Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
////                Translator.Culture = culture;
////                var actual = Translator.Translate(Properties.Resources.ResourceManager, key, arg0, arg1, arg2);
////                Assert.AreEqual(expected, actual);

////                Translator.ErrorHandling = ErrorHandling.Throw;
////                Translator.Culture = null;
////                actual = Translator.Translate(Properties.Resources.ResourceManager, key, culture, ErrorHandling.ReturnErrorInfo, arg0, arg1, arg2);
////                Assert.AreEqual(expected, actual);
////            }
////        }
////    }
////}