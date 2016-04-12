namespace Gu.Localization.Tests.Internals
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using NUnit.Framework;

    public class ResourceManagerExtTests
    {
        [Test]
        public void GetCultures()
        {
            var sw = Stopwatch.StartNew();
            var cultures = Properties.Resources.ResourceManager.GetCultures();
            Console.WriteLine($"Getting cultures took {sw.Elapsed.TotalMilliseconds.ToString("F2")} ms");
            Assert.AreEqual(new[] { "de", "en", "sv" }, cultures.Select(x => x.TwoLetterISOLanguageName));
        }
    }
}
