namespace Gu.Wpf.Localization.UiTests
{
    using System;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public sealed class TranslatesWithNeutralLanguage : IDisposable
    {
        private Application application;
        private Window window;
        private bool disposed;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.application?.Dispose();
            this.application = Application.AttachOrLaunch(StartInfo.WithNeutralLanguageProject);
            this.window = this.application.MainWindow();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.application?.Dispose();
        }

        [Test]
        public void EffectiveCulture()
        {
            Assert.AreEqual("en", this.window.FindTextBlock("CurrentCultureTextBlock").Text);
            this.window.FindRadioButton("pt").Click();
            Assert.AreEqual("pt", this.window.FindTextBlock("CurrentCultureTextBlock").Text);
        }

        [Test]
        public void VanillaXaml()
        {
            this.window.FindRadioButton("en").Click();
            var groupBox = this.window.FindGroupBox("Vanilla xaml");
            Assert.AreEqual("English", groupBox.FindTextBlock("AllLanguagesTextBlock").Text);

            this.window.FindRadioButton("pt").Click();
            Assert.AreEqual("Português", groupBox.FindTextBlock("AllLanguagesTextBlock").Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", groupBox.FindTextBlock("AllLanguagesTextBlock").Text);
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.application?.Dispose();
        }
    }
}