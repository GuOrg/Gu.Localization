namespace Gu.Wpf.Localization.UiTests
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    public static class StartInfo
    {
        public static ProcessStartInfo DemoProject { get; } = CreateStartUpInfo("Gu.Wpf.Localization.Demo", CultureInfo.GetCultureInfo("en"));

        public static ProcessStartInfo WithNeutralLanguageProject { get; } = CreateStartUpInfo("Gu.Wpf.Localization.WithNeutralLanguage", CultureInfo.GetCultureInfo("en"));

        private static ProcessStartInfo CreateStartUpInfo(string assemblyName, CultureInfo culture)
        {
            var processStartInfo = new ProcessStartInfo
            {
                Arguments = culture.Name,
                FileName = GetExeFileName(assemblyName),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            return processStartInfo;
        }

        private static string GetExeFileName(string assemblyName)
        {
            var testAssembnly = Assembly.GetExecutingAssembly();
            var testDirestory = Path.GetDirectoryName(new Uri(testAssembnly.CodeBase).AbsolutePath);
            var exeDirectory = testDirestory.Replace(testAssembnly.GetName().Name, assemblyName);
            var fileName = Path.Combine(exeDirectory, $"{assemblyName}.exe");
            return fileName;
        }
    }
}
