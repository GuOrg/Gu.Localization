namespace Gu.Localization.Analyzers.Tests.GULOC04UseCustomTranslateTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly CodeFixProvider Fix = new UseCustomTranslateFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("GULOC04");

        private static readonly string ResourcesCode = @"
namespace RoslynSandbox.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""System.Resources.Tools.StronglyTypedResourceBuilder"", ""15.0.0.0"")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute(""Microsoft.Performance"", ""CA1811:AvoidUncalledPrivateCode"")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager(""RoslynSandbox.Properties.Resources"", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value.
        /// </summary>
        public static string Key {
            get {
                return ResourceManager.GetString(""Key"", resourceCulture);
            }
        }
    }
}";

        private static readonly string TranslateCode = @"
namespace RoslynSandbox.Properties
{
    using Gu.Localization;

    public static class Translate
    {
        /// <summary>Call like this: Translate.Key(nameof(Resources.Saved_file__0_)).</summary>
        /// <param name=""key"">A key in Properties.Resources</param>
        /// <param name=""errorHandling"">How to handle translation errors like missing key or culture.</param>
        /// <returns>A translation for the key.</returns>
        public static string Key(string key, ErrorHandling errorHandling = ErrorHandling.ReturnErrorInfoPreserveNeutral)
        {
            return TranslationFor(key, errorHandling).Translated;
        }

        /// <summary>Call like this: Translate.Key(nameof(Resources.Saved_file__0_)).</summary>
        /// <param name=""key"">A key in Properties.Resources</param>
        /// <param name=""errorHandling"">How to handle translation errors like missing key or culture.</param>
        /// <returns>A translation for the key.</returns>
        public static ITranslation TranslationFor(string key, ErrorHandling errorHandling = ErrorHandling.ReturnErrorInfoPreserveNeutral)
        {
            return Gu.Localization.Translation.GetOrCreate(Resources.ResourceManager, key, errorHandling);
        }
    }
}";

        [Test]
        public static void TranslatorTranslateStringLiteralWithUsing()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = ↓Translator.Translate(Resources.ResourceManager, ""Key"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Translate.Key(""Key"");
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, TranslateCode, before }, after);
        }

        [Test]
        public static void TranslatorTranslateStringLiteralFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translate = ↓Translator.Translate(Properties.Resources.ResourceManager, ""Key"");
        }
    }
}";
            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Translate.Key(""Key"");
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, TranslateCode, before }, after);
        }

        [Test]
        public static void TranslationGetOrCreateStringLiteralWithUsing()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = ↓Translator.Translate(Resources.ResourceManager, ""Key"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Translate.Key(""Key"");
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, TranslateCode, before }, after);
        }

        [Test]
        public static void TranslationGetOrCreateStringLiteralFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translation = ↓Translation.GetOrCreate(Properties.Resources.ResourceManager, ""Key"");
        }
    }
}";
            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translation = Properties.Translate.TranslationFor(""Key"");
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, TranslateCode, before }, after);
        }

        [Test]
        public static void TranslationGetOrCreateNameofFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translation = ↓Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Key));
        }
    }
}";
            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translation = Properties.Translate.TranslationFor(nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, TranslateCode, before }, after);
        }
    }
}
