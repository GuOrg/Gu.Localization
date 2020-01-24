namespace Gu.Localization.Analyzers.Tests
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class HappyPathWithAll
    {
        private static readonly ImmutableArray<DiagnosticAnalyzer> AllAnalyzers = typeof(Gu.Localization.Analyzers.GULOC01KeyExists)
            .Assembly
            .GetTypes()
            .Where(typeof(DiagnosticAnalyzer).IsAssignableFrom)
            .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t))
            .ToImmutableArray();

        private static readonly Solution DemoProjectSolution = CodeFactory.CreateSolution(
            ProjectFile.Find("Gu.Wpf.Localization.Demo.csproj"),
            AllAnalyzers,
            MetadataReferences.FromAttributes());

        [Test]
        public void NotEmpty()
        {
            CollectionAssert.IsNotEmpty(AllAnalyzers);
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public void DemoProject(DiagnosticAnalyzer analyzer)
        {
            switch (analyzer)
            {
                case LiteralAnalyzer _:
                case MemberAccessAnalyzer _:
                case ResourceAnalyzer _:
                    // Just checking so that the analyzer does not throw here.
                    _ = Analyze.GetDiagnostics(analyzer, DemoProjectSolution);
                    break;
                default:
                    RoslynAssert.NoAnalyzerDiagnostics(analyzer, DemoProjectSolution);
                    break;
            }
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public void WithSyntaxErrors(DiagnosticAnalyzer analyzer)
        {
            var testCode = @"
    using System;
    using System.IO;

    public class Foo : SyntaxError
    {
        private readonly Stream stream = File.SyntaxError(string.Empty);
        private bool disposed;

        protected override void Dispose(bool disposing)
        {
            if (this.syntaxError)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.stream.Dispose();
            }

            base.Dispose(disposing);
        }
    }";
            RoslynAssert.NoAnalyzerDiagnostics(analyzer, testCode);
        }
    }
}
