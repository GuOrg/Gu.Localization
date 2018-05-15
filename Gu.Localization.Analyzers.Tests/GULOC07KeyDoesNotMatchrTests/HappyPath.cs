namespace Gu.Localization.Analyzers.Tests.GULOC07KeyDoesNotMatchrTests
{
    using System.IO;
    using Gu.Localization.Analyzers.Tests.Helpers;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    internal class HappyPath
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ResourceAnalyzer();
        private FileInfo projectFile;

        [SetUp]
        public void SetUp()
        {
            var original = ProjectFile.Find("Gu.Localization.TestStub.csproj");
            var tempDir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), original.Directory.Name));
            if (tempDir.Exists)
            {
                tempDir.Delete(recursive: true);
            }

            original.Directory.CopyTo(tempDir);
            this.projectFile = tempDir.FindFile(original.Name);
        }

        [Test]
        public void WhenValid()
        {
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            AnalyzerAssert.Valid(Analyzer, sln);
        }
    }
}
