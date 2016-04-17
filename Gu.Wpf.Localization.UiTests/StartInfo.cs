namespace Gu.Wpf.Localization.UiTests
{
    using System;
    using System.Diagnostics;

    using Gu.Wpf.Localization.Demo;

    public static class StartInfo
    {
        public static ProcessStartInfo DemoProject
        {
            get
            {
                var assembly = typeof(MainWindow).Assembly;
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = new Uri(assembly.CodeBase, UriKind.Absolute).LocalPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                return processStartInfo;
            }
        }
    }
}
