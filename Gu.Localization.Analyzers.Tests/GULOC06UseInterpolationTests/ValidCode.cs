namespace Gu.Localization.Analyzers.Tests.GULOC06UseInterpolationTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class ValidCode
    {
        private static readonly DiagnosticAnalyzer Analyzer = new LiteralAnalyzer();

        [Test]
        public void NoCuriles()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
#pragma warning disable GULOC03 Use resource.
            var translate = ""abc"";
#pragma warning restore GULOC03 Use resource.
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, testCode);
        }

        [Test]
        public void Interpolated()
        {
            var testCode = @"
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
            AnalyzerAssert.Valid(Analyzer, testCode);
        }
    }
}
