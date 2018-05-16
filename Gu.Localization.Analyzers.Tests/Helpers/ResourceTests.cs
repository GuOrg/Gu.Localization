namespace Gu.Localization.Analyzers.Tests.Helpers
{
    using NUnit.Framework;

    public class ResourceTests
    {
        [TestCase("Value", "Value")]
        [TestCase("Long 123456789abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRST123456789abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRST text", "Long_123456789abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRST123456789abcdefghijklmnopqrstuvxyzABCDEFG")]
        [TestCase("Some value", "Some_value")]
        [TestCase("Some\r\nvalue", "Some_n_value")]
        [TestCase("Some\nvalue", "Some_n_value")]
        [TestCase("The value is: {0}", "The_value_is____0__")]
        [TestCase("The first value is: {0} and the second value is {1}", "The_first_value_is____0___and_the_second_value_is___1__")]
        public void TryGetKey(string text, string expected)
        {
            if (expected == null)
            {
                Assert.AreEqual(false, Resources.TryGetKey(text, out _));
            }
            else
            {
                Assert.AreEqual(true, Resources.TryGetKey(text, out var actual));
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
