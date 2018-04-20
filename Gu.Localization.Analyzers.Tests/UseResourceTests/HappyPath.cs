namespace Gu.Localization.Analyzers.Tests.UseResourceTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    internal class HappyPath
    {
        private static readonly DiagnosticAnalyzer Analyzer = new LiteralAnalyzer();

        [Test]
        public void IgnoreInComparison()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Fo)
        {
            var translate = Translator.Translate(Resources.ResourceManager, nameof(Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, testCode);
        }
    }
}
