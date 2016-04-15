namespace Gu.Localization.Tests.Errors
{
    using System;
    using System.Globalization;
    using System.Text;

    using Gu.Localization.Errors;

    using NUnit.Framework;

    public class ResourceManagerErrorsTests
    {
        [Test]
        public void ForResourceManager()
        {
            var errors = ResourceManagerErrors.For(Properties.Resources.ResourceManager);
            Assert.IsFalse(errors.IsEmpty);
            CollectionAssert.AreEqual(new[] { "NeutralOnly", "EnglishOnly", "NoTranslation", "Value___0_" }, errors.Keys);
            var builder = new StringBuilder();
            builder.AppendLine("Key: NeutralOnly");
            builder.AppendLine("  Missing for: { de, en, sv }");
            builder.AppendLine("Key: EnglishOnly");
            builder.AppendLine("  Missing for: { de, sv }");
            builder.AppendLine("Key: NoTranslation");
            builder.AppendLine("  Missing for: { de, en, sv }");
            builder.AppendLine("Key: Value___0_");
            builder.AppendLine("  Has format errors, the formats are:");
            builder.AppendLine("    Value: {0}");
            builder.AppendLine("    null");
            builder.AppendLine("    Value: {0} {1}");
            builder.AppendLine("    Värde: ");
            builder.AppendLine("  Missing for: { de }");
            var expected = builder.ToString();
            var actual = errors.ToString("  ", Environment.NewLine);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ForEnum()
        {
            var errors = ResourceManagerErrors.ForEnum<DummyEnum>(Properties.Resources.ResourceManager);
            Assert.IsFalse(errors.IsEmpty);
            CollectionAssert.AreEqual(new[] { DummyEnum.MissingTranslation.ToString() }, errors.Keys);
            Assert.AreEqual("Key: MissingTranslation Missing for: { invariant, de, en, sv } ", errors.ToString("", " "));
        }

        [Test]
        public void FormatsHappyPath()
        {
            var errors = ResourceManagerErrors.For(Properties.Resources.ResourceManager, nameof(Properties.Resources.first___0___second__1_));
            CollectionAssert.IsEmpty(errors);

            errors = ResourceManagerErrors.For(Properties.Resources.ResourceManager, nameof(Properties.Resources.first___0___second__1_), new[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("en"), CultureInfo.GetCultureInfo("sv") });
            CollectionAssert.IsEmpty(errors);
        }

        [Test]
        public void FormatsWithErrors()
        {
            var errors = ResourceManagerErrors.For(Properties.Resources.ResourceManager, Properties.Resources.Value___0_);
            CollectionAssert.IsNotEmpty(errors);
        }

        [Test]
        public void ForKeyWhenNoErrors()
        {
            var errors = ResourceManagerErrors.For(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            CollectionAssert.IsEmpty(errors);

            errors = ResourceManagerErrors.For(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), new CultureInfo[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("en"), CultureInfo.GetCultureInfo("sv") });
            CollectionAssert.IsEmpty(errors);
        }
    }
}
