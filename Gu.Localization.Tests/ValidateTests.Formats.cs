namespace Gu.Localization.Tests
{
    using System;

    using NUnit.Framework;

    public partial class ValidateTests
    {
        public class Formats
        {
            [TestCase("Hej")]
            [TestCase("First: {0}, Second: {0}")]
            public void ThrowsOneArgument(string format)
            {
                Assert.Throws<FormatException>(() => Validate.Format(format, 1));
                Assert.IsFalse(format, Validate.IsValidFormat(format, 1));
            }
        }
    }
}
