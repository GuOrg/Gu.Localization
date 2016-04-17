namespace Gu.Wpf.Localization.UiTests
{
    using System.Diagnostics;

    using Gu.Wpf.Localization.Demo;

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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.application = Application.AttachOrLaunch(StartInfo.DemoProject);
            this.window = this.application.GetWindow("MainWindow");
            this.languageComboBox = this.window.Get<ComboBox>(AutomationIds.LanguagesComboBoxId);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.application?.Dispose();
        }

        [Test]
        public void VanillaXaml()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.VanillaXamlGroupId);

            this.languageComboBox.Select("sv");
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.languageComboBox.Select("en");
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);
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

            this.languageComboBox.Select("sv");
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);

            this.languageComboBox.Select("en");
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
        }

        [Test]
        public void UserControlSameProject()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.UserControlSameProjectGroupId);

            this.languageComboBox.Select("sv");
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);

            this.languageComboBox.Select("en");
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
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