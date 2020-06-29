namespace Gu.Localization.Analyzers.Tests.Helpers
{
    using System.IO;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class XamlFileTests
    {
        [TestCase("Bar", "<TextBlock Text=\"{x:Static p:Resources.Key}\"></TextBlock>", "<TextBlock Text=\"{x:Static p:Resources.Bar}\"></TextBlock>")]
        [TestCase("Bar", "<TextBlock Text=\"{x:Static p:Resources.Key}\" />", "<TextBlock Text=\"{x:Static p:Resources.Bar}\" />")]
        [TestCase("Bar", "<TextBlock><TextBlock.Text><x:Static Member=\"p:Resources.Key\" /></TextBlock.Text></TextBlock>", "<TextBlock><TextBlock.Text><x:Static Member=\"p:Resources.Bar\" /></TextBlock.Text></TextBlock>")]
        [TestCase("Bar", "<TextBlock Text=\"{Binding Path=(p:Resources.Key)}\" />", "<TextBlock Text=\"{Binding Path=(p:Resources.Bar)}\" />")]
        public void WhenSuccess(string newName, string before, string after)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox.Properties {
    using System;
    public class Resources {
        public static string Key {
            get {
                return ResourceManager.GetString(""Key"", resourceCulture);
            }
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var property = semanticModel.GetDeclaredSymbol(syntaxTree.FindPropertyDeclaration("Key"));
            string fileName = Path.Combine(Path.GetTempPath(), "Foo.xaml");
            var testXaml = @"
<UserControl x:Class=""Gu.Localization.TestStub.UserControl1""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" 
             xmlns:d=""http://schemas.microsoft.com/expression/blend/2008"" 
             xmlns:local=""clr-namespace:Gu.Localization.TestStub""
             xmlns:p=""clr-namespace:RoslynSandbox.Properties""
             mc:Ignorable=""d"" 
             d:DesignHeight=""450"" d:DesignWidth=""800"">
    <Grid>
        <TextBlock Text=""{x:Static p:Resources.Key}""></TextBlock>
    </Grid>
</UserControl>";
            testXaml = testXaml.AssertReplace("<TextBlock Text=\"{x:Static p:Resources.Key}\"></TextBlock>", before);
            File.WriteAllText(fileName, testXaml);
            Assert.AreEqual(true, XamlFile.TryUpdateUsage(fileName, property, newName, out var updated));
            var expected = @"
<UserControl x:Class=""Gu.Localization.TestStub.UserControl1""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" 
             xmlns:d=""http://schemas.microsoft.com/expression/blend/2008"" 
             xmlns:local=""clr-namespace:Gu.Localization.TestStub""
             xmlns:p=""clr-namespace:RoslynSandbox.Properties""
             mc:Ignorable=""d"" 
             d:DesignHeight=""450"" d:DesignWidth=""800"">
    <Grid>
        <TextBlock Text=""{x:Static p:Resources.Key}""></TextBlock>
    </Grid>
</UserControl>";
            expected = expected.AssertReplace("<TextBlock Text=\"{x:Static p:Resources.Key}\"></TextBlock>", after);
            Assert.AreEqual(expected, updated.Text);
            File.Delete(fileName);
        }

        [TestCase("Bar", "<TextBlock Text=\"{x:Static p:Resources.Key}\"></TextBlock>", "<TextBlock Text=\"{x:Static p:Resources.Bar}\"></TextBlock>")]
        [TestCase("Bar", "<TextBlock Text=\"{x:Static p:Resources.Key}\" />", "<TextBlock Text=\"{x:Static p:Resources.Bar}\" />")]
        public void WhenSuccessExplicitAssemblyName(string newName, string before, string after)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox.Properties {
    using System;
    public class Resources {
        public static string Key {
            get {
                return ResourceManager.GetString(""Key"", resourceCulture);
            }
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var property = semanticModel.GetDeclaredSymbol(syntaxTree.FindPropertyDeclaration("Key"));
            string fileName = Path.Combine(Path.GetTempPath(), "Foo.xaml");
            var testXaml = @"
<UserControl x:Class=""Gu.Localization.TestStub.UserControl1""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" 
             xmlns:d=""http://schemas.microsoft.com/expression/blend/2008"" 
             xmlns:local=""clr-namespace:Gu.Localization.TestStub""
             xmlns:p=""clr-namespace:RoslynSandbox.Properties;assembly=RoslynSandbox""
             mc:Ignorable=""d"" 
             d:DesignHeight=""450"" d:DesignWidth=""800"">
    <Grid>
        <TextBlock Text=""{x:Static p:Resources.Key}""></TextBlock>
    </Grid>
</UserControl>";
            testXaml = testXaml.AssertReplace("<TextBlock Text=\"{x:Static p:Resources.Key}\"></TextBlock>", before);
            File.WriteAllText(fileName, testXaml);
            Assert.AreEqual(true, XamlFile.TryUpdateUsage(fileName, property, newName, out var updated));
            var expected = @"
<UserControl x:Class=""Gu.Localization.TestStub.UserControl1""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" 
             xmlns:d=""http://schemas.microsoft.com/expression/blend/2008"" 
             xmlns:local=""clr-namespace:Gu.Localization.TestStub""
             xmlns:p=""clr-namespace:RoslynSandbox.Properties;assembly=RoslynSandbox""
             mc:Ignorable=""d"" 
             d:DesignHeight=""450"" d:DesignWidth=""800"">
    <Grid>
        <TextBlock Text=""{x:Static p:Resources.Key}""></TextBlock>
    </Grid>
</UserControl>";
            expected = expected.AssertReplace("<TextBlock Text=\"{x:Static p:Resources.Key}\"></TextBlock>", after);
            Assert.AreEqual(expected, updated.Text);
            File.Delete(fileName);
        }

        [TestCase("<TextBlock Text=\"{x:Static p:Resources.Key1}\"></TextBlock>")]
        public void WhenNoUsage(string element)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox.Properties {
    using System;
    public class Resources {
        public static string Key {
            get {
                return ResourceManager.GetString(""Key"", resourceCulture);
            }
        }

        public static string Key1 {
            get {
                return ResourceManager.GetString(""Key1"", resourceCulture);
            }
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var property = semanticModel.GetDeclaredSymbol(syntaxTree.FindPropertyDeclaration("Key"))!;
            string fileName = Path.Combine(Path.GetTempPath(), "Foo.xaml");
            var testXaml = @"
<UserControl x:Class=""Gu.Localization.TestStub.UserControl1""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" 
             xmlns:d=""http://schemas.microsoft.com/expression/blend/2008"" 
             xmlns:local=""clr-namespace:Gu.Localization.TestStub""
             xmlns:p=""clr-namespace:RoslynSandbox.Properties""
             mc:Ignorable=""d"" 
             d:DesignHeight=""450"" d:DesignWidth=""800"">
    <Grid>
        <TextBlock Text=""{x:Static p:Resources.Key1}""></TextBlock>
    </Grid>
</UserControl>";

            testXaml = testXaml.AssertReplace("<TextBlock Text=\"{x:Static p:Resources.Key1}\"></TextBlock>", element);
            File.WriteAllText(fileName, testXaml);
            Assert.AreEqual(false, XamlFile.TryUpdateUsage(fileName, property, null!, out _));
            File.Delete(fileName);
        }
    }
}
