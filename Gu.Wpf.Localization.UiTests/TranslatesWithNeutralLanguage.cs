namespace Gu.Wpf.Localization.UiTests
{
    using NUnit.Framework;
    using TestStack.White;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.WindowItems;

    public class TranslatesWithNeutralLanguage
    {
        private Application application;
        private Window window;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.application = Application.AttachOrLaunch(StartInfo.WithNeutralLanguageProject);
            this.window = this.application.GetWindow("MainWindow");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.application?.Dispose();
        }

        [Test]
        public void EffectiveCulture()
        {
            Assert.AreEqual("en", this.window.Get<Label>("CurrentCultureTextBlock").Text);
            this.window.Get<RadioButton>("pt").Click();
            Assert.AreEqual("pt", this.window.Get<Label>("CurrentCultureTextBlock").Text);
        }

        [Test]
        public void VanillaXaml()
        {
            this.window.Get<RadioButton>("en").Click();
            var groupBox = this.window.GetByText<GroupBox>("Vanilla xaml");
            Assert.AreEqual("English", groupBox.Get<Label>("AllLanguagesTextBlock").Text);

            this.window.Get<RadioButton>("pt").Click();
            Assert.AreEqual("Português", groupBox.Get<Label>("AllLanguagesTextBlock").Text);

            this.window.Get<RadioButton>("en").Click();
            Assert.AreEqual("English", groupBox.Get<Label>("AllLanguagesTextBlock").Text);
        }
    }
}