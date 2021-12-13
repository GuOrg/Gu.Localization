namespace Gu.Localization.Analyzers.Tests.GULOC10MissingTranslationTests
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Gu.Localization.Analyzers.Tests.Helpers;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class Diagnostic
    {
        private static readonly ResourceAnalyzer Analyzer = new();

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private FileInfo projectFile;
        private DirectoryInfo directory;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        [SetUp]
        public void SetUp()
        {
            var original = ProjectFile.Find("Gu.Localization.TestStub.csproj");
            this.directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), original.Directory!.Name));
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
            this.projectFile.Directory!.FindFile("Properties\\Resources.resx").ReplaceText("\"Key\"", "\"Value\"");
            this.projectFile.Directory!.FindFile("Properties\\Resources.sv.resx").ReplaceText("\"Key\"", "\"Value\"");
            this.projectFile.Directory!.FindFile("Properties\\Resources.Designer.cs").ReplaceText("public static string Key", "public static string Value");
            var sln = CodeFactory.CreateSolution(this.projectFile);
            var diagnostics = Analyze.GetDiagnostics(Analyzer, sln).Single().AnalyzerDiagnostics;
            Assert.AreEqual(1, diagnostics.Length);

            Assert.AreEqual("GULOC10", diagnostics[0].Id);
            Assert.AreEqual("The resource does not have translation to 'sv-SE', the neutral string is 'Value'", diagnostics[0].GetMessage(CultureInfo.InvariantCulture));
            StringAssert.EndsWith("Resources.Designer.cs", diagnostics[0].Location.SourceTree!.FilePath);
        }
    }
}
