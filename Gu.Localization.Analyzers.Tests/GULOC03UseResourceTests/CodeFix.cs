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

            expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <!-- 
    Microsoft ResX Schema 
    
    Version 2.0
    
    The primary goals of this format is to allow a simple XML format 
    that is mostly human readable. The generation and parsing of the 
    various data types are done through the TypeConverter classes 
    associated with the data types.
    
    Example:
    
    ... ado.net/XML headers & schema ...
    <resheader name=""resmimetype"">text/microsoft-resx</resheader>
    <resheader name=""version"">2.0</resheader>
    <resheader name=""reader"">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name=""writer"">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name=""Name1""><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name=""Color1"" type=""System.Drawing.Color, System.Drawing"">Blue</data>
    <data name=""Bitmap1"" mimetype=""application/x-microsoft.net.object.binary.base64"">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name=""Icon1"" type=""System.Drawing.Icon, System.Drawing"" mimetype=""application/x-microsoft.net.object.bytearray.base64"">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>
                
    There are any number of ""resheader"" rows that contain simple 
    name/value pairs.
    
    Each data row contains a name, and value. The row also contains a 
    type or mimetype. Type corresponds to a .NET class that support 
    text/value conversion through the TypeConverter architecture. 
    Classes that don't support this are serialized and stored with the 
    mimetype set.
    
    The mimetype is used for serialized objects, and tells the 
    ResXResourceReader how to depersist the object. This is currently not 
    extensible. For a given mimetype the value must be set accordingly:
    
    Note - application/x-microsoft.net.object.binary.base64 is the format 
    that the ResXResourceWriter will generate, however the reader can 
    read any of the formats listed below.
    
    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.
    
    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array 
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id=""root"" xmlns="""" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xsd:import namespace=""http://www.w3.org/XML/1998/namespace"" />
    <xsd:element name=""root"" msdata:IsDataSet=""true"">
      <xsd:complexType>
        <xsd:choice maxOccurs=""unbounded"">
          <xsd:element name=""metadata"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" use=""required"" type=""xsd:string"" />
              <xsd:attribute name=""type"" type=""xsd:string"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""assembly"">
            <xsd:complexType>
              <xsd:attribute name=""alias"" type=""xsd:string"" />
              <xsd:attribute name=""name"" type=""xsd:string"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""data"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
                <xsd:element name=""comment"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""2"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" msdata:Ordinal=""1"" />
              <xsd:attribute name=""type"" type=""xsd:string"" msdata:Ordinal=""3"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" msdata:Ordinal=""4"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""resheader"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name=""resmimetype"">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name=""version"">
    <value>2.0</value>
  </resheader>
  <resheader name=""reader"">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name=""writer"">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <data name=""Key"" xml:space=""preserve"">
    <value>Value</value>
  </data>
  <data name=""One_resource"" xml:space=""preserve"">
    <value>One resource</value>
  </data>
</root>".AssertReplace("One_resource", key).AssertReplace("One resource", value);
            CodeAssert.AreEqual(expected, fixedSln.FindAdditionalDocument("Designer.resx").GetText());
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

            expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <!-- 
    Microsoft ResX Schema 
    
    Version 2.0
    
    The primary goals of this format is to allow a simple XML format 
    that is mostly human readable. The generation and parsing of the 
    various data types are done through the TypeConverter classes 
    associated with the data types.
    
    Example:
    
    ... ado.net/XML headers & schema ...
    <resheader name=""resmimetype"">text/microsoft-resx</resheader>
    <resheader name=""version"">2.0</resheader>
    <resheader name=""reader"">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name=""writer"">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name=""Name1""><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name=""Color1"" type=""System.Drawing.Color, System.Drawing"">Blue</data>
    <data name=""Bitmap1"" mimetype=""application/x-microsoft.net.object.binary.base64"">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name=""Icon1"" type=""System.Drawing.Icon, System.Drawing"" mimetype=""application/x-microsoft.net.object.bytearray.base64"">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>
                
    There are any number of ""resheader"" rows that contain simple 
    name/value pairs.
    
    Each data row contains a name, and value. The row also contains a 
    type or mimetype. Type corresponds to a .NET class that support 
    text/value conversion through the TypeConverter architecture. 
    Classes that don't support this are serialized and stored with the 
    mimetype set.
    
    The mimetype is used for serialized objects, and tells the 
    ResXResourceReader how to depersist the object. This is currently not 
    extensible. For a given mimetype the value must be set accordingly:
    
    Note - application/x-microsoft.net.object.binary.base64 is the format 
    that the ResXResourceWriter will generate, however the reader can 
    read any of the formats listed below.
    
    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.
    
    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array 
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id=""root"" xmlns="""" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xsd:import namespace=""http://www.w3.org/XML/1998/namespace"" />
    <xsd:element name=""root"" msdata:IsDataSet=""true"">
      <xsd:complexType>
        <xsd:choice maxOccurs=""unbounded"">
          <xsd:element name=""metadata"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" use=""required"" type=""xsd:string"" />
              <xsd:attribute name=""type"" type=""xsd:string"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""assembly"">
            <xsd:complexType>
              <xsd:attribute name=""alias"" type=""xsd:string"" />
              <xsd:attribute name=""name"" type=""xsd:string"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""data"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
                <xsd:element name=""comment"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""2"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" msdata:Ordinal=""1"" />
              <xsd:attribute name=""type"" type=""xsd:string"" msdata:Ordinal=""3"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" msdata:Ordinal=""4"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""resheader"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name=""resmimetype"">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name=""version"">
    <value>2.0</value>
  </resheader>
  <resheader name=""reader"">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name=""writer"">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <data name=""Key"" xml:space=""preserve"">
    <value>Value</value>
  </data>
  <data name=""One_resource"" xml:space=""preserve"">
    <value>One resource</value>
  </data>
</root>".AssertReplace("One_resource", key).AssertReplace("One resource", value);
            CodeAssert.AreEqual(expected, fixedSln.FindAdditionalDocument("Designer.resx").GetText());
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

            expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <!-- 
    Microsoft ResX Schema 
    
    Version 2.0
    
    The primary goals of this format is to allow a simple XML format 
    that is mostly human readable. The generation and parsing of the 
    various data types are done through the TypeConverter classes 
    associated with the data types.
    
    Example:
    
    ... ado.net/XML headers & schema ...
    <resheader name=""resmimetype"">text/microsoft-resx</resheader>
    <resheader name=""version"">2.0</resheader>
    <resheader name=""reader"">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name=""writer"">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name=""Name1""><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name=""Color1"" type=""System.Drawing.Color, System.Drawing"">Blue</data>
    <data name=""Bitmap1"" mimetype=""application/x-microsoft.net.object.binary.base64"">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name=""Icon1"" type=""System.Drawing.Icon, System.Drawing"" mimetype=""application/x-microsoft.net.object.bytearray.base64"">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>
                
    There are any number of ""resheader"" rows that contain simple 
    name/value pairs.
    
    Each data row contains a name, and value. The row also contains a 
    type or mimetype. Type corresponds to a .NET class that support 
    text/value conversion through the TypeConverter architecture. 
    Classes that don't support this are serialized and stored with the 
    mimetype set.
    
    The mimetype is used for serialized objects, and tells the 
    ResXResourceReader how to depersist the object. This is currently not 
    extensible. For a given mimetype the value must be set accordingly:
    
    Note - application/x-microsoft.net.object.binary.base64 is the format 
    that the ResXResourceWriter will generate, however the reader can 
    read any of the formats listed below.
    
    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.
    
    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array 
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id=""root"" xmlns="""" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xsd:import namespace=""http://www.w3.org/XML/1998/namespace"" />
    <xsd:element name=""root"" msdata:IsDataSet=""true"">
      <xsd:complexType>
        <xsd:choice maxOccurs=""unbounded"">
          <xsd:element name=""metadata"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" use=""required"" type=""xsd:string"" />
              <xsd:attribute name=""type"" type=""xsd:string"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""assembly"">
            <xsd:complexType>
              <xsd:attribute name=""alias"" type=""xsd:string"" />
              <xsd:attribute name=""name"" type=""xsd:string"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""data"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
                <xsd:element name=""comment"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""2"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" msdata:Ordinal=""1"" />
              <xsd:attribute name=""type"" type=""xsd:string"" msdata:Ordinal=""3"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" msdata:Ordinal=""4"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""resheader"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name=""resmimetype"">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name=""version"">
    <value>2.0</value>
  </resheader>
  <resheader name=""reader"">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name=""writer"">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <data name=""Key"" xml:space=""preserve"">
    <value>Value</value>
  </data>
  <data name=""One_resource"" xml:space=""preserve"">
    <value>One resource</value>
  </data>
</root>".AssertReplace("One_resource", key).AssertReplace("One resource", value);
            CodeAssert.AreEqual(expected, fixedSln.FindAdditionalDocument("Designer.resx").GetText());
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
