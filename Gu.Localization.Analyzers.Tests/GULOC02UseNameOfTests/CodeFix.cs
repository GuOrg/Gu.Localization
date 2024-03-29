﻿namespace Gu.Localization.Analyzers.Tests.GULOC02UseNameOfTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly InvocationAnalyzer Analyzer = new();
        private static readonly UseNameOfFix Fix = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GULOC02UseNameOf);

        private const string ResourcesCode = @"
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
        public static void TranslatorTranslateStringLiteralWithUsing()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Translator.Translate(Resources.ResourceManager, ↓""Key"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Translator.Translate(Resources.ResourceManager, nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void TranslatorTranslateStringLiteralFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class C
    {
        public C()
        {
            var translate = Translator.Translate(Properties.Resources.ResourceManager, ↓""Key"");
        }
    }
}";
            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class C
    {
        public C()
        {
            var translate = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void TranslationGetOrCreateStringLiteralWithUsing()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Translator.Translate(Resources.ResourceManager, ↓""Key"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Translator.Translate(Resources.ResourceManager, nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void TranslationGetOrCreateStringLiteralFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class C
    {
        public C()
        {
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, ↓""Key"");
        }
    }
}";
            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class C
    {
        public C()
        {
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void TranslateKeyStringLiteralWithUsing()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Translate.Key(↓""Key"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Translate.Key(nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, TranslateCode, before }, after);
        }

        [Test]
        public static void TranslateKeyStringLiteralFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class C
    {
        public C()
        {
            var translate = Properties.Translate.Key(↓""Key"");
        }
    }
}";
            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class C
    {
        public C()
        {
            var translate = Properties.Translate.Key(nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, TranslateCode, before }, after);
        }

        [Test]
        public static void TranslateTranslationForLiteralWithUsing()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Translate.TranslationFor(↓""Key"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Translate.TranslationFor(nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, TranslateCode, before }, after);
        }

        [Test]
        public static void TranslateTranslationForStringLiteralFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class C
    {
        public C()
        {
            var translation = Properties.Translate.TranslationFor(↓""Key"");
        }
    }
}";
            var after = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class C
    {
        public C()
        {
            var translation = Properties.Translate.TranslationFor(nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, TranslateCode, before },  after);
        }

        [Test]
        public static void ResourceManagerGetObjectNameofPropertyWithUsing()
        {
            var before = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Resources.ResourceManager.GetObject(↓""Key"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Resources.ResourceManager.GetObject(nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void ResourceManagerGetObjectNameofPropertyFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    public class C
    {
        public C()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(↓""Key"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    public class C
    {
        public C()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void ResourceManagerGetObjectWithCultureNameofPropertyWithUsing()
        {
            var before = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Resources.ResourceManager.GetObject(↓""Key"", System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Resources.ResourceManager.GetObject(nameof(Resources.Key), System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void ResourceManagerGetObjectWithCultureNameofPropertyFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    public class C
    {
        public C()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(↓""Key"", System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    public class C
    {
        public C()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(nameof(Properties.Resources.Key), System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void ResourceManagerGetStringNameofPropertyWithUsing()
        {
            var before = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Resources.ResourceManager.GetString(↓""Key"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Resources.ResourceManager.GetString(nameof(Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void ResourceManagerGetStringNameofPropertyFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    public class C
    {
        public C()
        {
            var translate = Properties.Resources.ResourceManager.GetString(↓""Key"");
        }
    }
}";
            var after = @"
namespace RoslynSandbox
{
    public class C
    {
        public C()
        {
            var translate = Properties.Resources.ResourceManager.GetString(nameof(Properties.Resources.Key));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void ResourceManagerGetStringWithCultureNameofPropertyWithUsing()
        {
            var before = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Resources.ResourceManager.GetString(↓""Key"", System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            var after = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class C
    {
        public C()
        {
            var translate = Resources.ResourceManager.GetString(nameof(Resources.Key), System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }

        [Test]
        public static void ResourceManagerGetStringWithCultureNameofPropertyFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    public class C
    {
        public C()
        {
            var translate = Properties.Resources.ResourceManager.GetString(↓""Key"", System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            var after = @"
namespace RoslynSandbox
{
    public class C
    {
        public C()
        {
            var translate = Properties.Resources.ResourceManager.GetString(nameof(Properties.Resources.Key), System.Globalization.CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { ResourcesCode, before }, after);
        }
    }
}
