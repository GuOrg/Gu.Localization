// ReSharper disable UnusedVariable
// ReSharper disable AssignNullToNotNullAttribute

namespace Gu.Localization.Tests.Sandbox
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    public class TranslationsTests
    {
        [Test]
        public void GetOrCreate()
        {
            var sw = Stopwatch.StartNew();
            var translations = Sandbox.Translations.GetOrCreate(this.GetType().Assembly);
#if DEBUG
    Console.WriteLine($"Getting cultures took {sw.Elapsed.TotalMilliseconds.ToString("F2")} ms");
#endif

            Assert.AreEqual(Properties.Resources.ResourceManager.BaseName, translations.BaseName);
            CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, translations.Cultures.Select(x => x.TwoLetterISOLanguageName));
        }

        [Test]
        public void FindCultureFiles()
        {
            var sw = Stopwatch.StartNew();
            var assembly = this.GetType().Assembly;
            var uri = new Uri(assembly.CodeBase, UriKind.Absolute);
            var resourceName = $"{assembly.GetName().Name}.resources.dll";
            var files = Directory.EnumerateFiles(System.IO.Path.GetDirectoryName(uri.LocalPath), resourceName, SearchOption.AllDirectories).ToArray();
#if DEBUG
            Console.WriteLine($"Getting {files.Length} cultures took {sw.Elapsed.TotalMilliseconds.ToString("F2")} ms");
#endif

            ////CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, translations.Cultures.Select(x => x.TwoLetterISOLanguageName));
        }

        [Test]
        public void FindCultureFolders()
        {
            var sw = Stopwatch.StartNew();
            var assembly = this.GetType().Assembly;
            var uri = new Uri(assembly.CodeBase, UriKind.Absolute);
            var resourceName = $"{assembly.GetName().Name}.resources.dll";
            var directories = Directory.EnumerateDirectories(System.IO.Path.GetDirectoryName(uri.LocalPath)).ToArray();
            foreach (var directory in directories)
            {
                var count = Directory.EnumerateFiles(directory, resourceName, SearchOption.TopDirectoryOnly).Count();
            }
#if DEBUG
            Console.WriteLine($"Getting {directories.Length} cultures took {sw.Elapsed.TotalMilliseconds.ToString("F2")} ms");
#endif

            ////CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, translations.Cultures.Select(x => x.TwoLetterISOLanguageName));
        }
    }
}