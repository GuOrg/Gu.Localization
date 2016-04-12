namespace Gu.Localization.Benchmarks
{
    using System.Collections.Generic;
    using System.Globalization;

    using BenchmarkDotNet.Attributes;

    public class GetCulturesBenchmarks
    {
        [Benchmark]
        public IReadOnlyList<CultureInfo> GetCultures()
        {
            return Gu.Localization.Properties.Resources.ResourceManager.GetCultures();
        }
    }
}
