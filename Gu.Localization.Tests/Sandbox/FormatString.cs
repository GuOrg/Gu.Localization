using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gu.Localization.Tests.Sandbox
{
    using NUnit.Framework;

    public class FormatString
    {
        [TestCase("First: {0}")]
        public void FastParseFormat(string text)
        {
            int pos = 0;
            Assert.IsTrue(TrySkipPast(text, '{', ref pos));
            Assert.AreEqual('0', text[pos]);
            int value;
            Assert.IsTrue(TryParseUnsignedInt(text, ref pos, out value));
            Assert.AreEqual(0, value);
            Assert.IsTrue(TrySkipPast(text, '}', ref pos));
            Assert.AreEqual(pos, text.Length - 1);
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

        private static bool TrySkipPast(string text, char c, ref int pos)
        {
            while (pos < text.Length && text[pos] != c)
            {
                pos++;
            }

            if (pos == text.Length - 1)
            {
                return false;
            }

            pos++;
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
