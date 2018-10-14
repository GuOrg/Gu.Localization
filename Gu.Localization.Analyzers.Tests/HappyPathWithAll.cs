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
        private static readonly ImmutableArray<DiagnosticAnalyzer> AllAnalyzers = typeof(KnownSymbol)
            .Assembly
            .GetTypes()
            .Where(typeof(DiagnosticAnalyzer).IsAssignableFrom)
            .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t))
            .ToImmutableArray();

        private static readonly Solution DemoProjectSolution = CodeFactory.CreateSolution(
            ProjectFile.Find("Gu.Wpf.Localization.Demo.csproj"),
            AllAnalyzers,
            AnalyzerAssert.MetadataReferences);

        // ReSharper disable once InconsistentNaming
        private static readonly Solution AnalyzerProjectSolution = CodeFactory.CreateSolution(
            ProjectFile.Find("Gu.Localization.Analyzers.csproj"),
            AllAnalyzers,
            AnalyzerAssert.MetadataReferences);

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
                    AnalyzerAssert.NoAnalyzerDiagnostics(analyzer, DemoProjectSolution);
                    break;
            }
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public void AnalyzerProject(DiagnosticAnalyzer analyzer)
        {
            if (analyzer is LiteralAnalyzer)
            {
                // Just checking so that the analyzer does not throw here.
                _ = Analyze.GetDiagnostics(analyzer, AnalyzerProjectSolution);
            }
            else
            {
                AnalyzerAssert.Valid(analyzer, AnalyzerProjectSolution);
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
            AnalyzerAssert.NoAnalyzerDiagnostics(analyzer, testCode);
        }
    }
}
