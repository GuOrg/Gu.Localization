namespace Gu.Localization.Analyzers.Tests.GULOC06UseInterpolationTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new LiteralAnalyzer();
        private static readonly MakeInterpolatedFix Fix = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GULOC06UseInterpolation);

        [Test]
        public static void Interpolated()
        {
            var before = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = ↓""abc {1}"";
        }
    }
}";

            var after = @"
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
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }

        [Test]
        public static void InterpolatedVerbatim()
        {
            var before = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = ↓@""abc {1}"";
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = $@""abc {1}"";
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }
    }
}
