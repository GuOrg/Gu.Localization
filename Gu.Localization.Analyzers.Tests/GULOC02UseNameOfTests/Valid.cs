﻿namespace Gu.Localization.Analyzers.Tests.GULOC02UseNameOfTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();

        private const string ResourcesCode = @"namespace RoslynSandbox.Properties {
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

        private const string TranslateCode = @"
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
        public static void TranslatorTranslateNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Translator.Translate(Resources.ResourceManager, nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void TranslatorTranslateUnknownKey()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo(string key)
        {
            var translate = Translator.Translate(Resources.ResourceManager, key);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void TranslatorTranslateNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translate = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void TranslationGetOrCreateNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translation = Translation.GetOrCreate(Resources.ResourceManager, nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void TranslationGetOrCreateUnknownKey()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo(string key)
        {
            var translation = Translation.GetOrCreate(Resources.ResourceManager, key);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void TranslationGetOrCreateNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void TranslateKeyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Translate.Key(nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public static void TranslateUnknownKeyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class AppearanceItem
    {
        private readonly string translationKey;

        public AppearanceItem(string translationKey)
        {
            this.translationKey = translationKey;
        }

        public string DisplayName => Translate.Key(this.translationKey);
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public static void ResourceManagerGetObjectNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetObject(nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void ResourceManagerGetObjectNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void ResourceManagerGetObjectWithCultureNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetObject(nameof(Resources.Key), System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void ResourceManagerGetObjectWithCultureNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(nameof(Properties.Resources.Key), System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void ResourceManagerGetStringNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetString(nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void ResourceManagerGetStringNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetString(nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void ResourceManagerGetStringWithCultureNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetString(nameof(Resources.Key), System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void ResourceManagerGetStringWithCultureNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetString(nameof(Properties.Resources.Key), System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public static void ResourceManagerGetStringWithLiteralThatIsNotAKey()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
#pragma warning disable GULOC01 // Key does not exist
            var translate = Properties.Resources.ResourceManager.GetString(""Missing translation"", System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
#pragma warning restore GULOC01 // Key does not exist
        }
    }
}";
            RoslynAssert.Valid(Analyzer, ResourcesCode, testCode);
        }
    }
}
