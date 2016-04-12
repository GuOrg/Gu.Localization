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
    }
}