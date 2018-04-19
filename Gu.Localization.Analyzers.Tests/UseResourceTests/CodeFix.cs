namespace Gu.Localization.Analyzers.Tests.UseResourceTests
{
    using System.IO;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new LiteralAnalyzer();
        private static readonly CodeFixProvider Fix = new UseResourceFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("GULOC03");
        private FileInfo projectFile;

        [SetUp]
        public void SetUp()
        {
            var original = CodeFactory.FindProjectFile("Gu.Localization.TestStub.csproj");
            var tempDir = Path.Combine(Path.GetTempPath(), original.Directory.Name);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }

            Copy(original.Directory, new DirectoryInfo(tempDir));
            this.projectFile = new FileInfo(Path.Combine(tempDir, original.Name));
        }

        [Explicit]
        [Test]
        public void ForDebug()
        {
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            var diagnosticsAsync = Analyze.GetDiagnostics(sln, Analyzer);
            var fixedSln = Roslyn.Asserts.Fix.Apply(sln, Fix, diagnosticsAsync, fixTitle: "Move to resources.");
        }

        private static void Copy(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var dir in source.GetDirectories())
            {
                Copy(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (var file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name));
            }
        }
    }
}
