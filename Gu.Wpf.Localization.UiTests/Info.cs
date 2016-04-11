namespace Gu.Wpf.Localization.UiTests
{
    using System;
    using System.Diagnostics;

    using Gu.Wpf.Localization.Demo.WithResources;

    public static class Info
    {
        public static ProcessStartInfo ProcessStartInfo
        {
            get
            {
                var assembly = typeof(MainWindow).Assembly;
                var uri = new Uri(assembly.CodeBase, UriKind.Absolute);
                var fileName = uri.AbsolutePath;
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                return processStartInfo;
            }
        }
    }
}
