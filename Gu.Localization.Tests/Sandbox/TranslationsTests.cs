using System.IO;

namespace Gu.Localization.Tests.Sandbox
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using NUnit.Framework;

    public class TranslationsTests
    {
        [Test]
        public void GetOrCreate()
        {
            var sw = Stopwatch.StartNew();
            var translations = Sandbox.Translations.GetOrCreate(this.GetType().Assembly);
            Console.WriteLine($"Getting cultures took {sw.Elapsed.TotalMilliseconds.ToString("F2")} ms");
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
            Console.WriteLine($"Getting {files.Length} cultures took {sw.Elapsed.TotalMilliseconds.ToString("F2")} ms");
            //CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, translations.Cultures.Select(x => x.TwoLetterISOLanguageName));
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
                Directory.EnumerateFiles(directory, resourceName, SearchOption.TopDirectoryOnly).Any();
            }
            Console.WriteLine($"Getting {directories.Length} cultures took {sw.Elapsed.TotalMilliseconds.ToString("F2")} ms");
            //CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, translations.Cultures.Select(x => x.TwoLetterISOLanguageName));
        }
    }
}