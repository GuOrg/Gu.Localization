namespace Gu.Localization.Tests
{
    using System;
    using NUnit.Framework;

    public partial class ValidateTests
    {
        public class Formats
        {
            [TestCase("First: {0}")]
            [TestCase("First: {0:N}")]
            [TestCase("First: {0}, again: {0}")]
            [TestCase("First: {0:F2}, Second: {0:F3}")]
            public void OneArgumentHappyPath(string format)
            {
                Assert.DoesNotThrow(() => Validate.Format(format, 1));
                Assert.IsTrue(Validate.IsValidFormat(format, 1));
            }

            [TestCase("First: {0} Second: {1}")]
            [TestCase("First: {0:N}, second: {1:F3}")]
            [TestCase("First: {0}, again: {0:F2}, second: {1}")]
            [TestCase("First: {1}, again: {0:F2}, second: {0}")]
            public void TwoArgumentsHappyPath(string format)
            {
                Assert.DoesNotThrow(() => Validate.Format(format, 1, 2));
                Assert.IsTrue(Validate.IsValidFormat(format, 1, 2));
            }

            [TestCase("First: {0} second: {1}, third: {2}")]
            [TestCase("First: {0:N}, second: {1:F3}, third: {2:G}")]
            public void ParamsHappyPath(string format)
            {
                Assert.DoesNotThrow(() => Validate.Format(format, 1, 2, 3));
                Assert.IsTrue(Validate.IsValidFormat(format, 1, 2, 3));
            }

            [TestCase("Hej", "Invalid format string: \"Hej\" for the single argument: 1.")]
            [TestCase("First: {1}", "Invalid format string: \"First: {1}\".")]
            [TestCase("First: {0}, Second: {1}", "Invalid format string: \"First: {0}, Second: {1}\" for the single argument: 1.")]
            public void OneArgumentWithError(string format, string expected)
            {
                var exception = Assert.Throws<FormatException>(() => Validate.Format(format, 1));
                Assert.AreEqual(expected, exception.Message);
                Assert.IsFalse(Validate.IsValidFormat(format, 1));
            }

            [TestCase("Hej", "Invalid format string: \"Hej\" for the two arguments: 1, 2.")]
            [TestCase("First: {1}", "Invalid format string: \"First: {1}\".")]
            [TestCase("First: {0}, second: {1}, third: {2}", "Invalid format string: \"First: {0}, second: {1}, third: {2}\" for the two arguments: 1, 2.")]
            public void TwoArgumentsWithError(string format, string expected)
            {
                var exception = Assert.Throws<FormatException>(() => Validate.Format(format, 1, 2));
                Assert.AreEqual(expected, exception.Message);
                Assert.IsFalse(Validate.IsValidFormat(format, 1, 2));
            }

            [TestCase("Hej", "Invalid format string: \"Hej\" for the arguments: 1, 2, 3.")]
            [TestCase("First: {1}", "Invalid format string: \"First: {1}\".")]
            [TestCase("First: {0}, second: {1}, third: {2} {3}", "Invalid format string: \"First: {0}, second: {1}, third: {2} {3}\" for the arguments: 1, 2, 3.")]
            public void ParamsWithError(string format, string expected)
            {
                var exception = Assert.Throws<FormatException>(() => Validate.Format(format, 1, 2, 3));
                Assert.AreEqual(expected, exception.Message);
                Assert.IsFalse(Validate.IsValidFormat(format, 1, 2, 3));
            }
        }
    }
}
