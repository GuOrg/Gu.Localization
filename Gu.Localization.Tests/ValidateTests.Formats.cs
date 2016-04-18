namespace Gu.Localization.Tests
{
    using System;

    using NUnit.Framework;

    public partial class ValidateTests
    {
        public class Formats
        {
            [TestCase("Hej")]
            [TestCase("First: {1}")]
            [TestCase("First: {0}, Second: {1}")]
            public void OneArgumentWithError(string format)
            {
                Assert.Throws<FormatException>(() => Validate.Format(format, 1));
                Assert.IsFalse(Validate.IsValidFormat(format, 1));
            }

            [TestCase("First: {0}")]
            [TestCase("First: {0:N}")]
            [TestCase("First: {0}, Second: {0}")]
            [TestCase("First: {0:F2}, Second: {0:F3}")]
            public void OneArgumentHappyPath(string format)
            {
                Assert.DoesNotThrow(() => Validate.Format(format, 1));
                Assert.IsTrue(Validate.IsValidFormat(format, 1));
            }
        }
    }
}
