namespace Gu.Localization.Tests.Internals
{
    using System.Linq;

    using NUnit.Framework;

    public class FormatStringTests
    {
        [TestCase("first: {0}, second {1}", "0, 1")]
        [TestCase("first: {0:F2}, second {1}", "0, 1")]
        [TestCase("first: {{{0:F2}}}, second {{{1}}}", "0, 1")]
        [TestCase("second: {1}, first {0}", "1, 0")]
        [TestCase("second: {1}, first {0}, first: {0}", "1, 0, 0")]
        public void GetFormatItems(string format, string expecteds)
        {
            var expected = expecteds.Split(',')
                                    .Select(x => x.Trim())
                                    .ToArray();
            var actual = FormatString.GetFormatIndices(format);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestCase("0", true)]
        [TestCase("0, 0, 0", true)]
        [TestCase("1, 0, 0", true)]
        [TestCase("0, 0, 1", true)]
        [TestCase("1", false)]
        [TestCase("1, 1", false)]
        [TestCase("0, 2", false)]
        public void AreItemsValid(string itemsString, bool expected)
        {
            var items = itemsString.Split(',')
                                   .Select(x => x.Trim())
                                   .ToArray();
            var actual = FormatString.AreItemsValid(items);
            Assert.AreEqual(expected, actual);

            actual = FormatString.AreItemsValid(items);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AreItemsValidWhenEmpty()
        {
            Assert.IsTrue(FormatString.AreItemsValid(new string[0]));
        }

        [TestCase("0", 1)]
        [TestCase("0, 0, 0", 1)]
        [TestCase("1, 0, 0", 2)]
        [TestCase("0, 0, 1", 2)]
        public void Count(string itemsString, int expected)
        {
            var items = itemsString.Split(',')
                                   .Select(x => x.Trim())
                                   .ToArray();
            var actual = FormatString.CountUnique(items);
            Assert.AreEqual(expected, actual);

            actual = FormatString.CountUnique(items);
            Assert.AreEqual(expected, actual);
        }
    }
}
