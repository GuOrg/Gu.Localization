namespace Gu.Wpf.Localization.UiTests
{
    using Gu.Wpf.Localization.Demo;

    using NUnit.Framework;

    using TestStack.White;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.WindowItems;

    public class Translates
    {
        private Application application;
        private Window window;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.application = Application.AttachOrLaunch(StartInfo.DemoProject);
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
            Assert.AreEqual("en", this.window.Get<Label>(AutomationIds.CurrentCultureTextBlockId).Text);
            Assert.AreEqual("en", this.window.Get<Label>(AutomationIds.EffectiveCultureTextBlockId).Text);
            Assert.AreEqual("en", this.window.Get<TextBox>(AutomationIds.BoundCurrentCultureTextBoxId).Text);
            Assert.AreEqual("en", this.window.Get<Label>(AutomationIds.BoundEffectiveCultureTextBlockId).Text);

            this.window.Get<RadioButton>("sv").Click();
            Assert.AreEqual("sv", this.window.Get<Label>(AutomationIds.CurrentCultureTextBlockId).Text);
            Assert.AreEqual("sv", this.window.Get<Label>(AutomationIds.EffectiveCultureTextBlockId).Text);
            Assert.AreEqual("sv", this.window.Get<TextBox>(AutomationIds.BoundCurrentCultureTextBoxId).Text);
            Assert.AreEqual("sv", this.window.Get<Label>(AutomationIds.BoundEffectiveCultureTextBlockId).Text);
        }

        [Test]
        public void VanillaXaml()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.VanillaXamlGroupId);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.Get<RadioButton>("sv").Click();
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.Get<RadioButton>("en").Click();
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void VanillaXamlWithErrorHandling()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.VanillaXamlGroupWithReturnErrorInfoId);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.Get<RadioButton>("sv").Click();
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.Get<RadioButton>("en").Click();
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void NotInVisualTree()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.NotInVisualTreeGroupId);
            var dataGrid = groupBox.Get<ListView>(AutomationIds.DataGridId);

            this.window.Get<RadioButton>("sv").Click();
            Assert.AreEqual("Svenska", dataGrid.Header.Columns[0].Text);

            this.window.Get<RadioButton>("en").Click();
            Assert.AreEqual("English", dataGrid.Header.Columns[0].Text);
        }

        [Test]
        public void DataTemplate()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.DataTemplateGroupId);

            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.Get<RadioButton>("sv").Click();
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.Get<RadioButton>("en").Click();
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void UserControlSameProject()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.UserControlSameProjectGroupId);

            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.Get<RadioButton>("sv").Click();
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.Get<RadioButton>("en").Click();
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("", groupBox.Get<Label>(AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.Get<Label>(AutomationIds.EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void UserControlOtherProject()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.UserControlOtherProjectGroupId);
            var textBlock = groupBox.Get<Label>("KeyInControls");

            this.window.Get<RadioButton>("sv").Click();
            Assert.AreEqual("Från controls", textBlock.Text);

            this.window.Get<RadioButton>("en").Click();
            Assert.AreEqual("From Controls", textBlock.Text);
        }

        [Test, Explicit(Todo.FixThisTest)]
        public void CustomControlOtherProject()
        {
            // Assert.Inconclusive("Can't get KeyInControls");
            // maybe this is a white bug?
            var groupBox = this.window.Get<GroupBox>(AutomationIds.CustomControlOtherProjectGroupId);
            var textBlock = groupBox.Get<Label>("KeyInControls");

            this.window.Get<RadioButton>("sv").Click();
            Assert.AreEqual("Från controls", textBlock.Text);

            this.window.Get<RadioButton>("en").Click();
            Assert.AreEqual("From Controls", textBlock.Text);
        }

        [Test]
        public void NoTranslations()
        {
            var groupBox = this.window.Get<GroupBox>(AutomationIds.NoTranslationsGroupId);

            this.window.Get<RadioButton>("sv").Click();
            Assert.AreEqual("!MissingKey!", groupBox.Get<Label>(AutomationIds.MissingKeyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.Get<Label>(AutomationIds.BadFromatTextBlockId).Text);

            this.window.Get<RadioButton>("en").Click();
            Assert.AreEqual("!MissingKey!", groupBox.Get<Label>(AutomationIds.MissingKeyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.Get<Label>(AutomationIds.NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.Get<Label>(AutomationIds.BadFromatTextBlockId).Text);
        }

        [Test, Explicit(Todo.FixThisTest)]
        public void NoTranslationsWithReturnErrorInfo()
        {
            // looks like the static extension is called before the ErrorInfo.Mode has trickled down.
            // fixing this can be slightly messy
            var groupBox = this.window.Get<GroupBox>(AutomationIds.NoTranslationsWithReturnErrorInfoId);

            this.window.Get<RadioButton>("sv").Click();
            Assert.AreEqual("!MissingKey!", groupBox.Get<Label>(AutomationIds.MissingKeyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.Get<Label>(AutomationIds.NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.Get<Label>(AutomationIds.BadFromatTextBlockId).Text);

            this.window.Get<RadioButton>("en").Click();
            Assert.AreEqual("!MissingKey!", groupBox.Get<Label>(AutomationIds.MissingKeyTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.Get<Label>(AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.Get<Label>(AutomationIds.NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.Get<Label>(AutomationIds.BadFromatTextBlockId).Text);
        }
    }
}