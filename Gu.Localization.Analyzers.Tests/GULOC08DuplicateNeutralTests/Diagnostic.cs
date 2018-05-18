namespace Gu.Localization.Analyzers.Tests.GULOC08DuplicateNeutralTests
{
    using System.IO;
    using System.Linq;
    using Gu.Localization.Analyzers.Tests.Helpers;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    internal class Diagnostic
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ResourceAnalyzer();

        private FileInfo projectFile;
        private DirectoryInfo directory;

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
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            var diagnostics = Analyze.GetDiagnostics(sln, Analyzer).Single();
            Assert.AreEqual(4, diagnostics.Length);

            Assert.AreEqual("GULOC07", diagnostics[0].Id);
            StringAssert.EndsWith("Resources.Designer.cs", diagnostics[0].Location.SourceTree.FilePath);

            Assert.AreEqual("GULOC08", diagnostics[1].Id);
            StringAssert.EndsWith("Resources.Designer.cs", diagnostics[1].Location.SourceTree.FilePath);

            Assert.AreEqual("GULOC07", diagnostics[2].Id);
            StringAssert.EndsWith("Resources.Designer.cs", diagnostics[2].Location.SourceTree.FilePath);

            Assert.AreEqual("GULOC08", diagnostics[3].Id);
            StringAssert.EndsWith("Resources.Designer.cs", diagnostics[3].Location.SourceTree.FilePath);
        }
    }
}
