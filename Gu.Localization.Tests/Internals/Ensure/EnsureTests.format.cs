// ReSharper disable UnusedVariable
namespace Gu.Localization.Tests.Internals
{
    using System;

    using NUnit.Framework;

    public class EnsureTests
    {
        public static FormatData[] Valids => new[]
                                          {
                                              new FormatData(@"some string", null),
                                              new FormatData(@"some string", new object[0]),
                                              new FormatData(@"string with {0} parameter", new object[] { 1 }),
                                              new FormatData(@"string with {0} parameter {0} in to places", new object[] { 1 }),
                                              new FormatData(@"string with {0} parameter {1} in to places", new object[] { 2, 2 }),
                                              new FormatData(@"string with {0} parameter {1} in to places {0}", new object[] { 2, 2 }),
                                              new FormatData("string with {0} parameter {1} in {2} places", new object[] { 1, 2, 3 }),
                                          };

        public static FormatData[] InValids => new[]
                    {
                        new FormatData("some string", new object[] { 1 }),
                        new FormatData("string with {0} parameter", null),
                        new FormatData("string with {1} parameter", new object[] { 1 }),
                        new FormatData("string with {0} parameter {2}", new object[] { 1, 2 }),
                        new FormatData("string with {0} parameter", new object[0]),
                        new FormatData("string with {0} parameter", new object[] { 1, 2 }),
                        new FormatData("string with {0} parameter {0} in to places", null),
                        new FormatData("string with {0} parameter {0} in to places", new object[0]),
                        new FormatData("string with {0} parameter {0} in to places", new object[] { 1, 2 }),
                        new FormatData("string with {0} parameter {1} in to places", null),
                        new FormatData("string with {0} parameter {1} in to places", new object[0]),
                        new FormatData("string with {0} parameter {1} in to places", new object[] { 1 }),
                        new FormatData("string with {0} parameter {1} in to places", new object[] { 1, 2, 3 }),
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

        [TestCaseSource(nameof(InValids))]
        public void FormatThrows(FormatData data)
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.Format(data.Format, data.Args, "format", "args"));
#if DEBUG
            Console.WriteLine(ex.Message);
#endif
        }

        [TestCaseSource(nameof(InValids))]
        public void FormatDoesNotMatch(FormatData data)
        {
            Assert.False(Ensure.FormatMatches(data.Format, data.Args));
        }

        public class FormatData
        {
            public FormatData(string format, object[] args)
            {
                this.Format = format;
                this.Args = args;
            }

            public string Format { get; }

            public object[] Args { get; }

            public override string ToString()
            {
                var args = this.Args == null
                               ? "null"
                               : this.Args.Length == 0
                                     ? "object[0]"
                                     : string.Join(",", this.Args);
                return $"Format: {this.Format}, Args: {args}";
            }
        }
    }
}
