namespace Gu.Localization.Tests
{
    using System;
    using System.Globalization;
    using System.Text;

    using Gu.Localization.Tests.Errors;

    using NUnit.Framework;

    public partial class ValidateTests
    {
        public class Translations
        {
            [Test]
            public void ResourceManager()
            {
                var errors = Validate.Translations(Properties.Resources.ResourceManager);
                Assert.IsFalse(errors.IsEmpty);
                var expectedKeys = new[] { "InvalidFormat__0__", "NeutralOnly", "EnglishOnly", "NoTranslation" };
                CollectionAssert.AreEqual(expectedKeys, errors.Keys);
                var builder = new StringBuilder();
                builder.AppendLine("Key: InvalidFormat__0__")
                       .AppendLine("  Has format errors, the formats are:")
                       .AppendLine("    Value: {0}")
                       .AppendLine("    null")
                       .AppendLine("    Value: {0} {2}")
                       .AppendLine("    Värde: ")
                       .AppendLine("  Missing for: { de }")
                       .AppendLine("Key: NeutralOnly")
                       .AppendLine("  Missing for: { de, en, sv }")
                       .AppendLine("Key: EnglishOnly")
                       .AppendLine("  Missing for: { de, sv }")
                       .AppendLine("Key: NoTranslation")
                       .AppendLine("  Missing for: { de, en, sv }");
                var expected = builder.ToString();
                var actual = errors.ToString("  ", Environment.NewLine);
                Assert.AreEqual(expected, actual);
            }

            [Test]
            public void ResourceManagerExplicitCultures()
            {
                var cultures = new[] { CultureInfo.GetCultureInfo("sv"), CultureInfo.GetCultureInfo("en") };
                var errors = Validate.Translations(Properties.Resources.ResourceManager, cultures);
                Assert.IsFalse(errors.IsEmpty);
                var expectedKeys = new[] { "InvalidFormat__0__", "NeutralOnly", "EnglishOnly", "NoTranslation" };
                CollectionAssert.AreEqual(expectedKeys, errors.Keys);
                var builder = new StringBuilder();
                builder.AppendLine("Key: InvalidFormat__0__")
                       .AppendLine("  Has format errors, the formats are:")
                       .AppendLine("    Värde: ")
                       .AppendLine("    Value: {0} {2}")
                       .AppendLine("Key: NeutralOnly")
                       .AppendLine("  Missing for: { sv, en }")
                       .AppendLine("Key: EnglishOnly")
                       .AppendLine("  Missing for: { sv }")
                       .AppendLine("Key: NoTranslation")
                       .AppendLine("  Missing for: { sv, en }");
                var expected = builder.ToString();
                var actual = errors.ToString("  ", Environment.NewLine);
                Assert.AreEqual(expected, actual);
            }

            [Test]
            public void EnumTranslations()
            {
                var errors = Validate.EnumTranslations<DummyEnum>(Properties.Resources.ResourceManager);
                Assert.IsFalse(errors.IsEmpty);
                CollectionAssert.AreEqual(new[] { DummyEnum.MissingTranslation.ToString() }, errors.Keys);
                Assert.AreEqual("Key: MissingTranslation Missing for: { invariant, de, en, sv } ", errors.ToString("", " "));
            }

            [Test]
            public void EnumTranslationsExplicitCultures()
            {
                var cultures = new[] { CultureInfo.GetCultureInfo("sv"), CultureInfo.GetCultureInfo("en") };
                var errors = Validate.EnumTranslations<DummyEnum>(Properties.Resources.ResourceManager, cultures);
                Assert.IsFalse(errors.IsEmpty);
                CollectionAssert.AreEqual(new[] { DummyEnum.MissingTranslation.ToString() }, errors.Keys);
                Assert.AreEqual("Key: MissingTranslation Missing for: { sv, en } ", errors.ToString("", " "));
            }

            [TestCase(nameof(Properties.Resources.AllLanguages))]
            [TestCase(nameof(Properties.Resources.ValidFormat__0__))]
            [TestCase(nameof(Properties.Resources.ValidFormat__0__1__))]
            public void KeyWhenNoErrors(string key)
            {
                var resourceManager = Properties.Resources.ResourceManager;
                var errors = Validate.Translations(resourceManager, key);
                CollectionAssert.IsEmpty(errors);

                var cultures = new[]
                                   {
                                       CultureInfo.InvariantCulture,
                                       CultureInfo.GetCultureInfo("en"),
                                       CultureInfo.GetCultureInfo("sv")
                                   };
                errors = Validate.Translations(resourceManager, key, cultures);
                CollectionAssert.IsEmpty(errors);
            }

            [TestCase(nameof(Properties.Resources.InvalidFormat__0__))]
            [TestCase(nameof(Properties.Resources.EnglishOnly))]
            public void KeyWithErrors(string key)
            {
                var resourceManager = Properties.Resources.ResourceManager;
                var errors = Validate.Translations(resourceManager, key);
                CollectionAssert.IsNotEmpty(errors);
            }
        }
    }
}
