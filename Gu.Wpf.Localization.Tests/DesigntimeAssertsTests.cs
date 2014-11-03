namespace Gu.Wpf.Localization.Tests
{
    using System;

    using NUnit.Framework;

    public class DesigntimeAssertsTests
    {
        [Test]
        public void AssertTranslationThrowsWhenKeyIsMissing()
        {
            Assert.Throws<ArgumentException>(() => DesigntimeAsserts.AssertTranslation("MissingKey"));
        }

        [Test]
        public void DoesNotThrowWhenKeyIsPresent()
        {
            Assert.DoesNotThrow(() => DesigntimeAsserts.AssertTranslation("AllLanguages"));
        }
    }
}
