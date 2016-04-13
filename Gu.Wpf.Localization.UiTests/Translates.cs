namespace Gu.Wpf.Localization.UiTests
{
    using Gu.Wpf.Localization.Demo.WithResources;

    using NUnit.Framework;

    using TestStack.White;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.ListBoxItems;
    using TestStack.White.UIItems.WindowItems;

    public class Translates
    {
        private Application application;
        private Window window;

        private ComboBox languageComboBox;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.application = Application.AttachOrLaunch(Info.ProcessStartInfo);
            this.window = this.application.GetWindow("MainWindow");
            this.languageComboBox = this.window.Get<ComboBox>(AutomationIds.LanguagesComboBoxId);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            this.application?.Dispose();
        }

        [Test]
        public void VanillaXaml()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.VanillaXamlGroupId);
            var translatedToAllTextBlock = groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId);
            var swedishOnlyTextBlock = groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId);

            this.languageComboBox.Select("sv");
            Assert.AreEqual("Svenska", translatedToAllTextBlock.Text);
            Assert.AreEqual("Svenska", swedishOnlyTextBlock.Text);

            this.languageComboBox.Select("en");
            Assert.AreEqual("English", translatedToAllTextBlock.Text);
            Assert.AreEqual("", swedishOnlyTextBlock.Text);
        }

        [Test]
        public void NotInVisualTree()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.NotInVisualTreeGroupId);
            var dataGrid = groupBox.Get<ListView>(AutomationIds.DataGridId);

            this.languageComboBox.Select("sv");
            Assert.AreEqual("Svenska", dataGrid.Header.Columns[0].Text);

            this.languageComboBox.Select("en");
            Assert.AreEqual("English", dataGrid.Header.Columns[0].Text);
        }

        [Test]
        public void DataTemplate()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.DataTemplateGroupId);
            var translatedToAllTextBlock = groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId);
            var swedishOnlyTextBlock = groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId);

            this.languageComboBox.Select("sv");
            Assert.AreEqual("Svenska", translatedToAllTextBlock.Text);
            Assert.AreEqual("Svenska", swedishOnlyTextBlock.Text);

            this.languageComboBox.Select("en");
            Assert.AreEqual("English", translatedToAllTextBlock.Text);
            Assert.AreEqual("", swedishOnlyTextBlock.Text);
        }

        [Test]
        public void UserControlSameProject()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.UserControlSameProjectGroupId);
            var translatedToAllTextBlock = groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId);
            var swedishOnlyTextBlock = groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId);

            this.languageComboBox.Select("sv");
            Assert.AreEqual("Svenska", translatedToAllTextBlock.Text);
            Assert.AreEqual("Svenska", swedishOnlyTextBlock.Text);

            this.languageComboBox.Select("en");
            Assert.AreEqual("English", translatedToAllTextBlock.Text);
            Assert.AreEqual("", swedishOnlyTextBlock.Text);
        }

        [Test]
        public void UserControlOtherProject()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.UserControlOtherProjectGroupId);
            var textBlock = groupBox.Get<Label>("KeyInControls");

            this.languageComboBox.Select("sv");
            Assert.AreEqual("Från controls", textBlock.Text);

            this.languageComboBox.Select("en");
            Assert.AreEqual("From Controls", textBlock.Text);
        }

        [Test]
        public void CustomControlOtherProject()
        {
            Assert.Inconclusive();
            var groupBox = this.window.Get<GroupBox>(AutomationIds.CustomControlOtherProjectGroupId);
            var textBlock = groupBox.Get<Label>("KeyInControls");

            this.languageComboBox.Select("sv");
            Assert.AreEqual("Från controls", textBlock.Text);

            this.languageComboBox.Select("en");
            Assert.AreEqual("From Controls", textBlock.Text);
        }

        [Test]
        public void NoTranslations()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.NoTranslationsGroupId);

            this.languageComboBox.Select("sv");
            Assert.AreEqual("!MissingKey!", groupBox.Get<Label>(AutomationIds.MissingKeyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.Get<Label>(AutomationIds.BadFromatTextBlockId).Text);

            this.languageComboBox.Select("en");
            Assert.AreEqual("!MissingKey!", groupBox.Get<Label>(AutomationIds.MissingKeyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.Get<Label>(AutomationIds.BadFromatTextBlockId).Text);
        }
    }
}