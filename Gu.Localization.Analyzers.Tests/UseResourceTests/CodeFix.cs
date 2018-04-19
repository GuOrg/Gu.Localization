namespace Gu.Localization.Analyzers.Tests.UseResourceTests
{
    using System.IO;
    using Gu.Localization.Analyzers.Tests.Helpers;
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
        private FileInfo fooFile;
        private FileInfo designerFile;
        private FileInfo resxFile;

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
            Assert.AreEqual(true, this.projectFile.Exists);

            this.fooFile = new FileInfo(Path.Combine(tempDir, "Foo.cs"));
            Assert.AreEqual(true, this.fooFile.Exists);

            this.designerFile = new FileInfo(Path.Combine(tempDir, "Properties\\Resources.Designer.cs"));
            Assert.AreEqual(true, this.designerFile.Exists);

            this.resxFile = new FileInfo(Path.Combine(tempDir, "Properties\\Resources.resx"));
            Assert.AreEqual(true, this.resxFile.Exists);
        }

        [TestCase("One resource", "One_resource")]
        public void MoveToResource(string value, string key)
        {
            File.WriteAllText(this.fooFile.FullName, File.ReadAllText(this.fooFile.FullName).AssertReplace("One resource", value));
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            var diagnosticsAsync = Analyze.GetDiagnostics(sln, Analyzer);
            var fixedSln = Roslyn.Asserts.Fix.Apply(sln, Fix, diagnosticsAsync, fixTitle: "Move to resources.");
            var expected = @"namespace Gu.Localization.TestStub
{
    public class Foo
    {
        public Foo()
        {
            var text = Properties.Resources.One_resource;
        }
    }
}
".AssertReplace("One_resource", key);
            CodeAssert.AreEqual(expected, fixedSln.FindDocument("Foo.cs"));
            var property = $"internal static string {key} => ResourceManager.GetString(\"{key}\", resourceCulture);";
            StringAssert.Contains(property, File.ReadAllText(this.designerFile.FullName));
            var xml = $"  <data name=\"{key}\" xml:space=\"preserve\">\r\n" +
                      $"    <value>{value}</value>\r\n" +
                       "  </data>";
            StringAssert.Contains(xml, File.ReadAllText(this.resxFile.FullName));
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
