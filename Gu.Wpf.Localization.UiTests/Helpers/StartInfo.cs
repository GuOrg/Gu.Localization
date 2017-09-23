namespace Gu.Wpf.Localization.UiTests
{
    using System.Diagnostics;
    using System.Globalization;
    using Gu.Wpf.UiAutomation;

    public static class StartInfo
    {
        public static ProcessStartInfo DemoProject { get; } = CreateStartUpInfo(
            Application.FindExe("Gu.Wpf.Localization.Demo.exe"),
            CultureInfo.GetCultureInfo("en"));

        public static ProcessStartInfo WithNeutralLanguageProject { get; } = CreateStartUpInfo(
            Application.FindExe("Gu.Wpf.Localization.WithNeutralLanguage.exe"),
            CultureInfo.GetCultureInfo("en"));

        private static ProcessStartInfo CreateStartUpInfo(string exeFileName, CultureInfo culture)
        {
            var processStartInfo = new ProcessStartInfo
            {
                Arguments = culture.Name,
                FileName = exeFileName,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            return processStartInfo;
        }
    }
}
