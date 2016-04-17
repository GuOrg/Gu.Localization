namespace Gu.Localization.Tests.Internals
{
    using NUnit.Framework;

    public class FormatStringTests
    {
        [TestCase("", true, 0, false)]
        [TestCase("First", true, 0, false)]
        [TestCase("First: {{0}}", true, 0, false)]
        [TestCase("First: {{0}", false, -1, false)]
        [TestCase("First: {0}}", false, -1, false)]
        [TestCase("First: {0}", true, 1, false)]
        [TestCase("First: {{{0}}}", true, 1, false)]
        [TestCase("First: {{{0:F3}}}", true, 1, true)]
        [TestCase("First: {0:F2 }, Second: {0:N}", true, 1, true)]
        [TestCase("First: {0}, Second: {0:N}", true, 1, true)]
        [TestCase("First: {0}, Second: {1}", true, 2, false)]
        [TestCase("First: {1}, Second: {0}", true, 2, false)]
        [TestCase("First: {1}, Second: {2}", false, -1, false)]
        [TestCase("First: {0}, First: {0:N} Second: {1}", true, 2, true)]
        [TestCase("First: {0}, Second: {1:N}", true, 2, true)]
        [TestCase("First: {0:N}", true, 1, true)]
        [TestCase("First: {0} ", true, 1, false)]
        [TestCase("First: {1}", false, -1, false)]
        [TestCase("First: {0N}", false, -1, null)]
        public void IsValidFormat(string text, bool expected, int expectedIndex, bool? expectedFormat)
        {
            int count;
            bool? anyItemHasFormat;
            Assert.AreEqual(expected, FormatString.IsValidFormat(text, out count, out anyItemHasFormat));
            Assert.AreEqual(expectedIndex, count);
            Assert.AreEqual(expectedFormat, anyItemHasFormat);
        }
    }
}
