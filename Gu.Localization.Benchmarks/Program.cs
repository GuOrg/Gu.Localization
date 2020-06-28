// ReSharper disable All
#pragma warning disable 162
namespace Gu.Localization.Benchmarks
{
    using System;
    using System.IO;
    using System.Linq;
    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main()
        {
            if (false)
            {
                foreach (var summary in new BenchmarkSwitcher(typeof(Program).Assembly).RunAll())
                {
                    CopyResult(summary);
                }
            }
            else
            {
                CopyResult(BenchmarkRunner.Run<CultureBenchmark>());
            }
        }

        private static void CopyResult(Summary summary)
        {
            var sourceFileName = Directory.EnumerateFiles(summary.ResultsDirectoryPath, $"*{summary.Title}-report-github.md")
                .Single();
            var destinationFileName = Path.Combine(
                summary.ResultsDirectoryPath.Split(new[] { "\\bin\\" }, StringSplitOptions.RemoveEmptyEntries).First(),
                summary.Title.Split('.').Last() + ".md");
            Console.WriteLine($"Copy: {sourceFileName} -> {destinationFileName}");
            File.Copy(sourceFileName, destinationFileName, overwrite: true);
        }
    }
}
