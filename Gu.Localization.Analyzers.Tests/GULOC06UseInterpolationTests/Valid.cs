namespace Gu.Localization.Analyzers.Tests.GULOC06UseInterpolationTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new LiteralAnalyzer();

        [Test]
        public static void NoCuriles()
        {
            var code = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
#pragma warning disable GULOC03 Use resource
            var translate = ""abc"";
#pragma warning restore GULOC03 Use resource
        }
    }
}";
            RoslynAssert.Valid(Analyzer, code);
        }

        [Test]
        public static void Interpolated()
        {
            var code = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = $""abc {1}"";
        }
    }
}";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}
