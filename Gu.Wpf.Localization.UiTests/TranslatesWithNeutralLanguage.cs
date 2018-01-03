namespace Gu.Wpf.Localization.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public sealed class TranslatesWithNeutralLanguage
    {
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(StartInfo.WithNeutralLanguageProject.FileName);
        }

        [Test]
        public void EffectiveCulture()
        {
            using (var application = Application.AttachOrLaunch(StartInfo.WithNeutralLanguageProject))
            {
                var window = application.MainWindow;
                window.FindRadioButton("en").IsChecked = true;
                Assert.AreEqual("en", window.FindTextBlock("CurrentCultureTextBlock").Text);
                window.FindRadioButton("pt").IsChecked = true;
                Assert.AreEqual("pt", window.FindTextBlock("CurrentCultureTextBlock").Text);
            }
        }

        [Test]
        public void VanillaXaml()
        {
            using (var application = Application.AttachOrLaunch(StartInfo.WithNeutralLanguageProject))
            {
                var window = application.MainWindow;
                window.FindRadioButton("en").IsChecked = true;
                var groupBox = window.FindGroupBox("Vanilla xaml");
                Assert.AreEqual("English", groupBox.FindTextBlock("AllLanguagesTextBlock").Text);

                window.FindRadioButton("pt").IsChecked = true;
                Assert.AreEqual("Português", groupBox.FindTextBlock("AllLanguagesTextBlock").Text);

                window.FindRadioButton("en").IsChecked = true;
                Assert.AreEqual("English", groupBox.FindTextBlock("AllLanguagesTextBlock").Text);
            }
        }
    }
}
