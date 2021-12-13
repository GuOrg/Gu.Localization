namespace Gu.Localization.Analyzers.Tests.GULOC08DuplicateNeutralTests
{
    using System.IO;
    using System.Linq;
    using Gu.Localization.Analyzers.Tests.Helpers;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class Diagnostic
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ResourceAnalyzer();

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private FileInfo projectFile;
        private DirectoryInfo directory;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        [SetUp]
        public void SetUp()
        {
            var original = ProjectFile.Find("Gu.Localization.TestStub.csproj");
            this.directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), original.Directory.Name));
            if (this.directory.Exists)
            {
                this.directory.Delete(recursive: true);
            }

            original.Directory.CopyTo(this.directory);
            this.projectFile = this.directory.FindFile(original.Name);
        }

        [Test]
        public void WhenDupe()
        {
            this.directory.FindFile("Properties\\Resources.resx").ReplaceText(
                "<value>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua</value>",
                $"<value>Value</value>");
            var sln = CodeFactory.CreateSolution(this.projectFile);
            var diagnostics = Analyze.GetDiagnostics(Analyzer, sln).Single();
            Assert.AreEqual(4, diagnostics.AnalyzerDiagnostics.Length);

            CollectionAssert.AreEquivalent(new[] { "GULOC07", "GULOC07", "GULOC08", "GULOC08" }, diagnostics.AnalyzerDiagnostics.Select(x => x.Id).ToArray());
            CollectionAssert.AreEquivalent(new[] { "Resources.Designer.cs", "Resources.Designer.cs", "Resources.Designer.cs", "Resources.Designer.cs" }, diagnostics.AnalyzerDiagnostics.Select(x => Path.GetFileName(x.Location.SourceTree!.FilePath)).ToArray());
        }
    }
}
