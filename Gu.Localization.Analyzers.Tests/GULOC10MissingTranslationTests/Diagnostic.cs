namespace Gu.Localization.Analyzers.Tests.GULOC10MissingTranslationTests
{
    using System.Globalization;
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
        public void WhenMissing()
        {
            this.projectFile.Directory.FindFile("Properties\\Resources.resx").ReplaceText("\"Key\"", "\"Value\"");
            this.projectFile.Directory.FindFile("Properties\\Resources.sv.resx").ReplaceText("\"Key\"", "\"Value\"");
            this.projectFile.Directory.FindFile("Properties\\Resources.Designer.cs").ReplaceText("public static string Key", "public static string Value");
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            var diagnostics = Analyze.GetDiagnostics(sln, Analyzer).Single();
            Assert.AreEqual(1, diagnostics.Length);

            Assert.AreEqual("GULOC10", diagnostics[0].Id);
            Assert.AreEqual("The resource does not have translation to 'sv-SE', the neutral string is 'Value'", diagnostics[0].GetMessage(CultureInfo.InvariantCulture));
            StringAssert.EndsWith("Resources.Designer.cs", diagnostics[0].Location.SourceTree.FilePath);
        }
    }
}
