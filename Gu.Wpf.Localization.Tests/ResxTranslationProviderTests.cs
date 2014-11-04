//namespace Gu.Wpf.Localization.Tests
//{
//    using System;
//    using System.Globalization;
//    using System.Linq;
//    using System.Threading;
//    using NUnit.Framework;

//    public class ResxTranslationProviderTests
//    {
//        private ResxTranslationProvider _provider;
//        [SetUp]
//        public void SetUp()
//        {
//            _provider = new ResxTranslationProvider(Properties.Resources.ResourceManager);
//        }

//        [Test]
//        public void Languages()
//        {
//            var expected = new[]
//            {
//                CultureInfo.GetCultureInfo("en"),
//                CultureInfo.GetCultureInfo("sv")
//            };
//            var actual = _provider.Languages;
//            Console.WriteLine("actual: {{{0}}}", string.Join(", ", actual.Select(x => x.TwoLetterISOLanguageName)));
//            CollectionAssert.AreEqual(expected, actual);
//        }

//        [TestCase("AllLanguages", "en", "English")]
//        [TestCase("AllLanguages", "sv", "Svenska")]
//        [TestCase("Missing", "sv", "-Missing-")]
//        [TestCase("EnglishOnly", "sv", "-EnglishOnly-")]
//        public void Translate(string key, string culture, string expected)
//        {
//            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
//            var actual = _provider.Translate(key);
//            Assert.AreEqual(expected, actual);
//        }

//        [TestCase("en", true)]
//        [TestCase("sv", true)]
//        [TestCase("no", false)]
//        public void HasCulture(string cultureName, bool expected)
//        {
//            var culture = CultureInfo.GetCultureInfo(cultureName);
//            Assert.AreEqual(expected, _provider.HasCulture(culture));
//        }

//        [TestCase("AllLanguages", "en", true)]
//        [TestCase("AllLanguages", "sv", true)]
//        [TestCase("AllLanguages", "no", false)]
//        public void HasKey(string key, string cultureName, bool expected)
//        {
//            var culture = CultureInfo.GetCultureInfo(cultureName);
//            Assert.AreEqual(expected, _provider.HasKey(key, culture));
//        }

//        [TestCase("AllLanguages", "en", true)]
//        [TestCase("KeyInControls", "en", true)]
//        public void HasKeyManyResources(string key, string cultureName, bool expected)
//        {
//            var culture = CultureInfo.GetCultureInfo(cultureName);
//            var provider = new ResxTranslationProvider(Properties.Resources.ResourceManager, Gu.Wpf.Localization.Demo.Controls.Properties.Resources.ResourceManager);
//            //var provider = new ResxTranslationProvider(Gu.Wpf.Localization.Demo.Controls.Properties.Resources.ResourceManager);
//            Assert.AreEqual(expected, provider.HasKey(key, culture));
//        }
//    }
//}
