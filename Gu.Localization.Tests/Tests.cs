using System;
using System.Linq;

namespace Gu.Localization.Tests.Sandbox
{
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Resources;

    using NUnit.Framework;

    public class Tests
    {
        [TestCase("en", true)]
        [TestCase("en-US", true)]
        [TestCase("sv", true)]
        [TestCase("no", false)]
        public void GetResourceSet(string cultureName, bool exists)
        {
            var stopwatch = Stopwatch.StartNew();
            var resourceSet = Properties.Resources.ResourceManager.GetResourceSet(new CultureInfo(cultureName), true, true);
            stopwatch.Stop();
            Console.WriteLine("Took: {0} ms", stopwatch.ElapsedMilliseconds);
            if (exists)
            {
                Assert.NotNull(resourceSet);
            }
            else
            {
                Assert.IsNull(resourceSet);
            }
        }

        [Test]
        public void GetCultures()
        {
            var stopwatch = Stopwatch.StartNew();
            var assembly = GetType().Assembly;
            var codeBase = assembly.CodeBase;
            var uri = new Uri(codeBase, UriKind.Absolute);
            var directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(uri.LocalPath));
            var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(uri.LocalPath);
            var resourceFiles = Directory.EnumerateFiles(directory.FullName, fileNameWithoutExtension + ".resources.dll", SearchOption.AllDirectories)
                                         .Select(x => new FileInfo(x))
                                         .ToArray();
            foreach (var file in resourceFiles)
            {
                var cultureInfo = new CultureInfo(file.Directory.Name);
                Console.WriteLine(cultureInfo.NativeName);
            }
            Console.WriteLine("Took: {0} ms", stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void ResourceSets()
        {
            var assembly = GetType().Assembly;
            var codeBase = assembly.CodeBase;
            var uri = new Uri(codeBase, UriKind.Absolute);
            var directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(uri.LocalPath));
            var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(uri.LocalPath);
            var resourceFiles = Directory.EnumerateFiles(directory.FullName, fileNameWithoutExtension + ".resources.dll", SearchOption.AllDirectories)
                                         .Select(x => new FileInfo(x))
                                         .ToArray();
            var resourceSets = resourceFiles.Select(x => new ResourceSet(x.FullName))
                                            .ToArray();
        }

        [Test]
        public void FileBased()
        {
            var assembly = GetType().Assembly;
            var codeBase = assembly.CodeBase;
            var uri = new Uri(codeBase, UriKind.Absolute);
            var directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(uri.LocalPath));
            var resourceManager = ResourceManager.CreateFileBasedResourceManager(assembly.FullName, directory.FullName, null);
            var resourceSet = resourceManager.GetResourceSet(new CultureInfo("en-US"), true, false);
        }
    }
}
