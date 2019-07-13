namespace Gu.Localization.Analyzers.Tests.GULOC07KeyDoesNotMatchTests
{
    using System.IO;
    using Gu.Localization.Analyzers.Tests.Helpers;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class ValidCode
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

        [TestCase("Value", "Value")]
        [TestCase("Resources_", "Resources")]
        public void WhenValid(string key, string value)
        {
            this.projectFile.Directory.FindFile("Properties\\Resources.resx").ReplaceText("\"Key\"", $"\"{key}\"");
            this.projectFile.Directory.FindFile("Properties\\Resources.resx").ReplaceText("<value>Value</value>", $"<value>{value}</value>");
            this.projectFile.Directory.FindFile("Properties\\Resources.sv.resx").ReplaceText("\"Key\"", $"\"{key}\"");
            this.projectFile.Directory.FindFile("Properties\\Resources.sv-SE.resx").ReplaceText("\"Key\"", $"\"{key}\"");
            this.projectFile.Directory.FindFile("Properties\\Resources.Designer.cs").ReplaceText("public static string Key", $"public static string {key}");
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            RoslynAssert.NoAnalyzerDiagnostics(Analyzer, sln);
        }
    }
}
