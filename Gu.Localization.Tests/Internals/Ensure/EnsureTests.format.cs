// ReSharper disable UnusedVariable
namespace Gu.Localization.Tests.Internals
{
    using System;

    using NUnit.Framework;

    public class EnsureTests
    {
        private static readonly FormatData[] Valids = new[]
        {
            new FormatData("No argument", null),
            new FormatData("No argument", Array.Empty<object>()),
            new FormatData("{0}", new object[] { 1 }),
            new FormatData("{0} {0}", new object[] { 1 }),
            new FormatData("{0} {1}", new object[] { 2, 2 }),
            new FormatData("{0} {1} {0}", new object[] { 2, 2 }),
            new FormatData("{0} {1} {2}", new object[] { 1, 2, 3 }),
        };

        private static readonly FormatData[] Invalids = new[]
        {
            new FormatData("some string", new object[] { 1 }),
            new FormatData("{0}", null),
            new FormatData("{1}", new object[] { 1 }),
            new FormatData("{0} {2}", new object[] { 1, 2 }),
            new FormatData("{0}", Array.Empty<object>()),
            new FormatData("{0}", new object[] { 1, 2 }),
            new FormatData("{0} {0}", null),
            new FormatData("{0} {0}", Array.Empty<object>()),
            new FormatData("{0} {0}", new object[] { 1, 2 }),
            new FormatData("{0} {1}", null),
            new FormatData("{0} {1}", Array.Empty<object>()),
            new FormatData("{0} {1}", new object[] { 1 }),
            new FormatData("{0} {1}", new object[] { 1, 2, 3 }),
        };

        [TestCaseSource(nameof(Valids))]
        public void FormatHappyPath(FormatData data)
        {
            Assert.DoesNotThrow(() => Ensure.Format(data.Format, data.Args, "format", "args"));
        }

        [TestCaseSource(nameof(Valids))]
        public void FormatMatchesHappyPath(FormatData data)
        {
            Assert.True(Ensure.FormatMatches(data.Format, data.Args));
        }

        [TestCaseSource(nameof(Invalids))]
        public void FormatThrows(FormatData data)
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.Format(data.Format, data.Args, "format", "args"));
#if DEBUG
            Console.WriteLine(ex.Message);
#endif
        }

        [TestCaseSource(nameof(Invalids))]
        public void FormatDoesNotMatch(FormatData data)
        {
            Assert.False(Ensure.FormatMatches(data.Format, data.Args));
        }

        public class FormatData
        {
            public FormatData(string format, object[]? args)
            {
                this.Format = format;
                this.Args = args;
            }

            public string Format { get; }

            public object[]? Args { get; }

            public override string ToString()
            {
                var args = this.Args is null
                               ? "null"
                               : this.Args.Length == 0
                                     ? "object[0]"
                                     : string.Join(",", this.Args);
                return $"Format: {this.Format}, Args: {args}";
            }
        }
    }
}
