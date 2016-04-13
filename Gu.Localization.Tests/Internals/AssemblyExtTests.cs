namespace Gu.Localization.Tests.Internals
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using NUnit.Framework;

    public class AssemblyExtTests
    {
        [SetUp]
        public void SetUp()
        {
            // touching it to warm it up for more relevant timings
            Culture.Exists("en");
        }

        [Test]
        public void AllCultures()
        {
            var sw = Stopwatch.StartNew();
            var assembly = this.GetType().Assembly;
            var culturesAndFileNames = assembly.AllCultures();
            Console.WriteLine($"Getting {culturesAndFileNames.Count} cultures took {sw.Elapsed.TotalMilliseconds.ToString("F2")} ms");
            CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, culturesAndFileNames.Select(x => x.TwoLetterISOLanguageName));
        }

        [Test]
        public void FindCultureFiles()
        {
            var sw = Stopwatch.StartNew();
            var assembly = this.GetType().Assembly;
            var culturesAndFileNames = assembly.CulturesAndFileNames();
            Console.WriteLine($"Getting {culturesAndFileNames.Count} cultures took {sw.Elapsed.TotalMilliseconds.ToString("F2")} ms");
            CollectionAssert.AreEquivalent(new[] { "de", "en", "sv" }, culturesAndFileNames.Keys.Select(x => x.TwoLetterISOLanguageName));
            //CollectionAssert.AreEqual(new[] { "de", "en", "sv" }, culturesAndFileNames.Values);
        }
    }
}
