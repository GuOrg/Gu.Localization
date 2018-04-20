namespace Gu.Localization.Analyzers.Tests.GULOC03UseResourceTests
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
        private FileInfo projectFile;
        private FileInfo fooFile;
        private FileInfo designerFile;
        private FileInfo resxFile;

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
            this.fooFile = tempDir.FindFile("Foo.cs");
            this.designerFile = tempDir.FindFile("Properties\\Resources.Designer.cs");
            this.resxFile = tempDir.FindFile("Properties\\Resources.resx");
        }

        [TestCase("One resource", "One_resource")]
        [TestCase("One resource.", "One_resource_")]
        [TestCase("abc", "abc")]
        [TestCase("0", "_0")]
        [TestCase("One {0}", "One___0__")]
        [TestCase("One {0} Two {1}", "One___0___Two___1__")]
        public void MoveToResource(string value, string key)
        {
            this.fooFile.ReplaceText("One resource", value);
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            var diagnosticsAsync = Analyze.GetDiagnostics(sln, Analyzer);
            var fixedSln = Roslyn.Asserts.Fix.Apply(sln, Fix, diagnosticsAsync, fixTitle: $"Move to Properties.Resources.{key}.");
            var expected = @"// ReSharper disable UnusedMember.Global
#pragma warning disable 219
namespace Gu.Localization.TestStub
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

        [TestCase("One resource", "One_resource")]
        [TestCase("One resource.", "One_resource_")]
        [TestCase("abc", "abc")]
        [TestCase("0", "_0")]
        [TestCase("One {0}", "One___0__")]
        [TestCase("One {0} Two {1}", "One___0___Two___1__")]
        public void MoveToResourceAndUseTranslatorTranslate(string value, string key)
        {
            this.fooFile.ReplaceText("One resource", value);
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            var diagnosticsAsync = Analyze.GetDiagnostics(sln, Analyzer);
            var fixedSln = Roslyn.Asserts.Fix.Apply(sln, Fix, diagnosticsAsync, fixTitle: $"Move to Properties.Resources.{key} and use Translator.Translate.");
            var expected = @"// ReSharper disable UnusedMember.Global
#pragma warning disable 219
namespace Gu.Localization.TestStub
{
    public class Foo
    {
        public Foo()
        {
            var text = Gu.Localization.Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.One_resource));
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

        [TestCase("One resource", "One_resource")]
        [TestCase("One resource.", "One_resource_")]
        [TestCase("abc", "abc")]
        [TestCase("0", "_0")]
        [TestCase("One {0}", "One___0__")]
        [TestCase("One {0} Two {1}", "One___0___Two___1__")]
        public void MoveToResourceAndUseTranslateKey(string value, string key)
        {
            this.fooFile.ReplaceText("One resource", value);
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            var diagnosticsAsync = Analyze.GetDiagnostics(sln, Analyzer);
            var fixedSln = Roslyn.Asserts.Fix.Apply(sln, Fix, diagnosticsAsync, fixTitle: $"Move to Properties.Resources.{key} and use Properties.Translate.Key(Properties.Resources.{key}).");
            var expected = @"// ReSharper disable UnusedMember.Global
#pragma warning disable 219
namespace Gu.Localization.TestStub
{
    public class Foo
    {
        public Foo()
        {
            var text = Properties.Translate.Key(nameof(Properties.Resources.One_resource));
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

        [Test]
        public void UseExistingResource()
        {
            File.WriteAllText(this.fooFile.FullName, File.ReadAllText(this.fooFile.FullName).AssertReplace("One resource", "Key"));
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            var diagnosticsAsync = Analyze.GetDiagnostics(sln, Analyzer);
            var fixedSln = Roslyn.Asserts.Fix.Apply(sln, Fix, diagnosticsAsync, fixTitle: $"Use existing Properties.Resources.Key.");
            var expected = @"// ReSharper disable UnusedMember.Global
#pragma warning disable 219
namespace Gu.Localization.TestStub
{
    public class Foo
    {
        public Foo()
        {
            var text = Properties.Resources.Key;
        }
    }
}
";
            CodeAssert.AreEqual(expected, fixedSln.FindDocument("Foo.cs"));
        }

        [Test]
        public void UseExistingInTranslatorTranslate()
        {
            File.WriteAllText(this.fooFile.FullName, File.ReadAllText(this.fooFile.FullName).AssertReplace("One resource", "Key"));
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            var diagnosticsAsync = Analyze.GetDiagnostics(sln, Analyzer);
            var fixedSln = Roslyn.Asserts.Fix.Apply(sln, Fix, diagnosticsAsync, fixTitle: $"Use existing Properties.Resources.Key in Translator.Translate");
            var expected = @"// ReSharper disable UnusedMember.Global
#pragma warning disable 219
namespace Gu.Localization.TestStub
{
    public class Foo
    {
        public Foo()
        {
            var text = Gu.Localization.Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Key));
        }
    }
}
";
            CodeAssert.AreEqual(expected, fixedSln.FindDocument("Foo.cs"));
        }

        [Test]
        public void UseExistingInTranslateKey()
        {
            File.WriteAllText(this.fooFile.FullName, File.ReadAllText(this.fooFile.FullName).AssertReplace("One resource", "Key"));
            var sln = CodeFactory.CreateSolution(this.projectFile, MetadataReferences.FromAttributes());
            var diagnosticsAsync = Analyze.GetDiagnostics(sln, Analyzer);
            var fixedSln = Roslyn.Asserts.Fix.Apply(sln, Fix, diagnosticsAsync, fixTitle: $"Use existing Properties.Resources.Key in Properties.Translate.Key(Properties.Resources.Key)");
            var expected = @"// ReSharper disable UnusedMember.Global
#pragma warning disable 219
namespace Gu.Localization.TestStub
{
    public class Foo
    {
        public Foo()
        {
            var text = Properties.Translate.Key(nameof(Properties.Resources.Key));
        }
    }
}
";
            CodeAssert.AreEqual(expected, fixedSln.FindDocument("Foo.cs"));
        }
    }
}
