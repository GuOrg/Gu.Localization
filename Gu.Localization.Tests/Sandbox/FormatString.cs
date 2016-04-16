namespace Gu.Localization.Tests.Sandbox
{
    using NUnit.Framework;

    public class FormatString
    {
        [TestCase("First: {0}", 0, null)]
        [TestCase("First: {0:N}", 0, "N")]
        [TestCase("First: {0} ", 0, null)]
        [TestCase("First: {1}", 1, null)]
        public void FastParseFormat(string text, int expectedIndex, string expectedFormat)
        {
            int pos = 0;
            Assert.IsTrue(TrySkipTo(text, '{', ref pos));
            Assert.AreEqual('{', text[pos]);
            int index;
            string format;
            Assert.IsTrue(TryParseFormat(text, ref pos, out index, out format));
            Assert.AreEqual(expectedIndex, index);
            Assert.AreEqual(expectedFormat, format);
            Assert.IsFalse(TrySkipTo(text, '{', ref pos));
            Assert.AreEqual(pos, text.Length);
        }

        [Test]
        public void TryParseTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                var pos = 0;
                int actual;
                Assert.IsTrue(TryParseUnsignedInt(i.ToString(), ref pos, out actual));
                Assert.AreEqual(i, actual);
            }
        }

        private static bool TrySkipTo(string text, char c, ref int pos)
        {
            while (pos < text.Length && text[pos] != c)
            {
                pos++;
            }

            return pos < text.Length && text[pos] == c;
        }

        private static bool TryParseFormat(string text, ref int pos, out int index, out string format)
        {
            if (text[pos] != '{')
            {
                index = -1;
                format = null;
                return false;
            }

            pos++;
            if (!TryParseUnsignedInt(text, ref pos, out index))
            {
                format = null;
                return false;
            }

            TryParseFormatSuffix(text, ref pos, out format);
            if (!TrySkipTo(text, '}', ref pos))
            {
                index = -1;
                format = null;
                return false;
            }

            pos++;
            return true;
        }

        private static bool TryParseFormatSuffix(string text, ref int pos, out string result)
        {
            if (text[pos] != ':')
            {
                result = null;
                return false;
            }

            if (pos < text.Length - 1 && text[pos + 1] == '}')
            {
                result = null;
                return false;
            }

            pos++;
            var start = pos;
            if (!TrySkipTo(text, '}', ref pos))
            {
                result = null;
                return false;
            }

            result = text.Slice(start, pos - 1);
            return true;
        }

        private static bool TryParseUnsignedInt(string text, ref int pos, out int result)
        {
            result = -1;
            while (pos < text.Length)
            {
                var i = text[pos] - '0';
                if (i < 0 || i > 9)
                {
                    return result != -1;
                }

                if (result == -1)
                {
                    result = i;
                }
                else
                {
                    result *= 10;
                    result += i;
                }
                pos++;
            }

            return result != -1;
        }
    }
}
