// ReSharper disable UnusedVariable
namespace Gu.Localization.Tests.Internals
{
    using System;

    using NUnit.Framework;

    public class EnsureTests
    {
        private static readonly TestCaseData[] Valids = new[]
        {
            new TestCaseData("No argument", null),
            new TestCaseData("No argument", Array.Empty<object>()),
            new TestCaseData("{0}", new object[] { 1 }),
            new TestCaseData("{0} {0}", new object[] { 1 }),
            new TestCaseData("{0} {1}", new object[] { 2, 2 }),
            new TestCaseData("{0} {1} {0}", new object[] { 2, 2 }),
            new TestCaseData("{0} {1} {2}", new object[] { 1, 2, 3 }),
        };

        private static readonly TestCaseData[] Invalids = new[]
        {
            new TestCaseData("some string", new object[] { 1 }),
            new TestCaseData("{0}", null),
            new TestCaseData("{1}", new object[] { 1 }),
            new TestCaseData("{0} {2}", new object[] { 1, 2 }),
            new TestCaseData("{0}", Array.Empty<object>()),
            new TestCaseData("{0}", new object[] { 1, 2 }),
            new TestCaseData("{0} {0}", null),
            new TestCaseData("{0} {0}", Array.Empty<object>()),
            new TestCaseData("{0} {0}", new object[] { 1, 2 }),
            new TestCaseData("{0} {1}", null),
            new TestCaseData("{0} {1}", Array.Empty<object>()),
            new TestCaseData("{0} {1}", new object[] { 1 }),
            new TestCaseData("{0} {1}", new object[] { 1, 2, 3 }),
        };

        [TestCaseSource(nameof(Valids))]
        public void FormatHappyPath(string format, object[] args)
        {
            Assert.DoesNotThrow(() => Ensure.Format(format, args, "format", "args"));
        }

        [TestCaseSource(nameof(Valids))]
        public void FormatMatchesHappyPath(string format, object[] args)
        {
            Assert.True(Ensure.FormatMatches(format, args));
        }

        [TestCaseSource(nameof(Invalids))]
        public void FormatThrows(string format, object[] args)
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.Format(format, args, "format", "args"));
#if DEBUG
            Console.WriteLine(ex!.Message);
#endif
        }

        [TestCaseSource(nameof(Invalids))]
        public void FormatDoesNotMatch(string format, object[] args)
        {
            Assert.False(Ensure.FormatMatches(format, args));
        }
    }
}
