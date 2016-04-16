namespace Gu.Localization.Tests.Internals
{
    using NUnit.Framework;

    public class StringExtTests
    {
        [TestCase("abcd", 0, 3, "abcd")]
        [TestCase("abcd", 1, 3, "bcd")]
        [TestCase("abcd", 0, 0, "a")]
        [TestCase("abcd", 3, 3, "d")]
        public void Slice(string text, int start, int end, string expected)
        {
            var actual = text.Slice(start, end);
            Assert.AreEqual(expected, actual);
        }
    }
}
