namespace Gu.Wpf.Localization.UiTests
{
    using System;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public sealed class TranslatesDemoProject : IDisposable
    {
        private Application application;
        private Window window;
        private bool disposed;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.application?.Dispose();
            this.application = Application.AttachOrLaunch(StartInfo.DemoProject);
            this.window = this.application.MainWindow();
        }

        [SetUp]
        public void SetUp()
        {
            this.window.FindRadioButton("en").Click();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.application?.Dispose();
        }

        [Test]
        public void EffectiveCulture()
        {
            Assert.AreEqual("en", this.window.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.CurrentCultureTextBlockId).Text);
            Assert.AreEqual("en", this.window.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EffectiveCultureTextBlockId).Text);
            Assert.AreEqual("en", this.window.FindTextBox(Gu.Wpf.Localization.Demo.AutomationIds.BoundCurrentCultureTextBoxId).Text);
            Assert.AreEqual("en", this.window.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.BoundEffectiveCultureTextBlockId).Text);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("sv", this.window.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.CurrentCultureTextBlockId).Text);
            Assert.AreEqual("sv", this.window.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EffectiveCultureTextBlockId).Text);
            Assert.AreEqual("sv", this.window.FindTextBox(Gu.Wpf.Localization.Demo.AutomationIds.BoundCurrentCultureTextBoxId).Text);
            Assert.AreEqual("sv", this.window.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.BoundEffectiveCultureTextBlockId).Text);
        }

        [Test]
        public void VanillaXaml()
        {
            var groupBox = this.window.FindGroupBox(Gu.Wpf.Localization.Demo.AutomationIds.VanillaXamlGroupId);
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void VanillaXamlWithErrorHandling()
        {
            var groupBox = this.window.FindGroupBox(Gu.Wpf.Localization.Demo.AutomationIds.VanillaXamlGroupWithReturnErrorInfoId);
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void NotInVisualTree()
        {
            var groupBox = this.window.FindGroupBox(Gu.Wpf.Localization.Demo.AutomationIds.NotInVisualTreeGroupId);
            var dataGrid = groupBox.FindDataGrid(Gu.Wpf.Localization.Demo.AutomationIds.DataGridId);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Svenska", dataGrid.Header.Columns[0].Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", dataGrid.Header.Columns[0].Text);
        }

        [Test]
        public void DataTemplate()
        {
            var groupBox = this.window.FindGroupBox(Gu.Wpf.Localization.Demo.AutomationIds.DataTemplateGroupId);

            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void UserControlSameProject()
        {
            var groupBox = this.window.FindGroupBox(Gu.Wpf.Localization.Demo.AutomationIds.UserControlSameProjectGroupId);

            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void UserControlOtherProject()
        {
            var groupBox = this.window.FindGroupBox("UserControl from other project");
            var textBlock = groupBox.FindTextBlock("KeyInControls");

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Från controls", textBlock.Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("From Controls", textBlock.Text);
        }

        [Test]
        [Explicit(Todo.FixThisTest)]
        public void CustomControlOtherProject()
        {
            // Assert.Inconclusive("Can't get KeyInControls");
            // maybe this is a white bug?
            var groupBox = this.window.FindGroupBox(Gu.Wpf.Localization.Demo.AutomationIds.CustomControlOtherProjectGroupId);
            var textBlock = groupBox.FindLabel("KeyInControls");

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Från controls", textBlock.Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("From Controls", textBlock.Text);
        }

        [Test]
        public void NoTranslations()
        {
            var groupBox = this.window.FindGroupBox(Gu.Wpf.Localization.Demo.AutomationIds.NoTranslationsGroupId);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("!MissingKey!", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.MissingKeyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.BadFromatTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("!MissingKey!", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.MissingKeyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.BadFromatTextBlockId).Text);
        }

        [Test]
        [Explicit(Todo.FixThisTest)]
        public void NoTranslationsWithReturnErrorInfo()
        {
            // looks like the static extension is called before the ErrorInfo.Mode has trickled down.
            // fixing this can be slightly messy
            var groupBox = this.window.FindGroupBox(Gu.Wpf.Localization.Demo.AutomationIds.NoTranslationsWithReturnErrorInfoId);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("!MissingKey!", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.MissingKeyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.BadFromatTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("!MissingKey!", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.MissingKeyTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindLabel(Gu.Wpf.Localization.Demo.AutomationIds.BadFromatTextBlockId).Text);
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