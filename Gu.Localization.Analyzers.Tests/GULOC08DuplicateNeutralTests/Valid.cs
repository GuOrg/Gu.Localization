namespace Gu.Localization.Analyzers.Tests.GULOC08DuplicateNeutralTests
{
    using System.IO;
    using Gu.Localization.Analyzers.Tests.Helpers;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ResourceAnalyzer();
        private static FileInfo projectFile;

        [SetUp]
        public static void SetUp()
        {
            var original = ProjectFile.Find("Gu.Localization.TestStub.csproj");
            var tempDir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), original.Directory.Name));
            if (tempDir.Exists)
            {
                tempDir.Delete(recursive: true);
            }

            original.Directory.CopyTo(tempDir);
            projectFile = tempDir.FindFile(original.Name);
        }

        [Test]
        public static void WhenValid()
        {
            projectFile.Directory.FindFile("Properties\\Resources.resx").ReplaceText("\"Key\"", "\"Value\"");
            projectFile.Directory.FindFile("Properties\\Resources.sv.resx").ReplaceText("\"Key\"", "\"Value\"");
            projectFile.Directory.FindFile("Properties\\Resources.sv-SE.resx").ReplaceText("\"Key\"", "\"Value\"");
            projectFile.Directory.FindFile("Properties\\Resources.Designer.cs").ReplaceText("public static string Key", "public static string Value");
            var solution = CodeFactory.CreateSolution(projectFile, MetadataReferences.FromAttributes());
            RoslynAssert.NoAnalyzerDiagnostics(Analyzer, solution);
        }
    }
}
