namespace Gu.Wpf.Localization.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class TranslatesWithNeutralLanguage
    {
        [Test]
        public static void EffectiveCulture()
        {
            // Just a smoke test so we don't crash.
            using var app = Application.Launch("Gu.Wpf.Localization.WithNeutralLanguage.exe");
            Assert.NotNull(app.MainWindow);
        }
    }
}
