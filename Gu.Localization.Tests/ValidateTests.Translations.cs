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
            public void ForResourceManager()
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
                       .AppendLine("    Value: {0} {1}")
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
            public void TranslationsForResourceManagerExplicitCultures()
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
                       .AppendLine("    Value: {0} {1}")
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
                Assert.AreEqual(
                    "Key: MissingTranslation Missing for: { invariant, de, en, sv } ",
                    errors.ToString("", " "));
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

            [Test]
            public void FormatsHappyPath()
            {
                var errors = Validate.Translations(
                    Properties.Resources.ResourceManager,
                    nameof(Properties.Resources.first___0___second__1_));
                CollectionAssert.IsEmpty(errors);

                var cultures = new[]
                                   {
                                       CultureInfo.InvariantCulture,
                                       CultureInfo.GetCultureInfo("en"),
                                       CultureInfo.GetCultureInfo("sv")
                                   };
                errors = Validate.Translations(
                    Properties.Resources.ResourceManager,
                    nameof(Properties.Resources.first___0___second__1_),
                    cultures);
                CollectionAssert.IsEmpty(errors);
            }

            [Test]
            public void FormatsWithErrors()
            {
                var errors = Validate.Translations(
                    Properties.Resources.ResourceManager,
                    Properties.Resources.InvalidFormat__0__);
                CollectionAssert.IsNotEmpty(errors);
            }

            [Test]
            public void TranslationsForKeyWhenNoErrors()
            {
                var errors = Validate.Translations(
                    Properties.Resources.ResourceManager,
                    nameof(Properties.Resources.AllLanguages));
                CollectionAssert.IsEmpty(errors);

                var cultures = new[]
                                   {
                                       CultureInfo.InvariantCulture,
                                       CultureInfo.GetCultureInfo("en"),
                                       CultureInfo.GetCultureInfo("sv")
                                   };
                errors = Validate.Translations(
                    Properties.Resources.ResourceManager,
                    nameof(Properties.Resources.AllLanguages),
                    cultures);
                CollectionAssert.IsEmpty(errors);
            }
        }
    }
}
