namespace Gu.Wpf.Localization.UiTests
{
    using System;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;
    using static Gu.Wpf.Localization.Demo.AutomationIds;

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
            this.window = this.application.MainWindow;
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
            Assert.AreEqual("en", this.window.FindTextBlock(CurrentCultureTextBlockId).Text);
            Assert.AreEqual("en", this.window.FindTextBlock(EffectiveCultureTextBlockId).Text);
            Assert.AreEqual("en", this.window.FindTextBox(BoundCurrentCultureTextBoxId).Text);
            Assert.AreEqual("en", this.window.FindTextBlock(BoundEffectiveCultureTextBlockId).Text);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("sv", this.window.FindTextBlock(CurrentCultureTextBlockId).Text);
            Assert.AreEqual("sv", this.window.FindTextBlock(EffectiveCultureTextBlockId).Text);
            Assert.AreEqual("sv", this.window.FindTextBox(BoundCurrentCultureTextBoxId).Text);
            Assert.AreEqual("sv", this.window.FindTextBlock(BoundEffectiveCultureTextBlockId).Text);
        }

        [Test]
        public void VanillaXaml()
        {
            var groupBox = this.window.FindGroupBox(VanillaXamlGroupId);
            Assert.AreEqual("English", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void VanillaXamlWithErrorHandling()
        {
            var groupBox = this.window.FindGroupBox(VanillaXamlGroupWithReturnErrorInfoId);
            Assert.AreEqual("English", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void NotInVisualTree()
        {
            var groupBox = this.window.FindGroupBox(NotInVisualTreeGroupId);
            var dataGrid = groupBox.FindDataGrid(DataGridId);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Svenska", dataGrid.ColumnHeaders[0].Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", dataGrid.ColumnHeaders[0].Text);
        }

        [Test]
        public void DataTemplate()
        {
            var groupBox = this.window.FindGroupBox(DataTemplateGroupId);

            Assert.AreEqual("English", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);
        }

        [Test]
        public void UserControlSameProject()
        {
            var groupBox = this.window.FindGroupBox(UserControlSameProjectGroupId);

            Assert.AreEqual("English", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("English", groupBox.FindTextBlock(TranslatedToAllTextBlockId).Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock(SwedishOnlyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("English", groupBox.FindTextBlock(EnumTranslatedToAllTextBlockId).Text);
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
            var groupBox = this.window.FindGroupBox(CustomControlOtherProjectGroupId);
            var textBlock = groupBox.FindTextBlock("KeyInControls");

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("Från controls", textBlock.Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("From Controls", textBlock.Text);
        }

        [Test]
        public void NoTranslations()
        {
            var groupBox = this.window.FindGroupBox(NoTranslationsGroupId);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("!MissingKey!", groupBox.FindTextBlock(MissingKeyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock(NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindTextBlock(BadFromatTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("!MissingKey!", groupBox.FindTextBlock(MissingKeyTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock(NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindTextBlock(BadFromatTextBlockId).Text);
        }

        [Test]
        [Explicit(Todo.FixThisTest)]
        public void NoTranslationsWithReturnErrorInfo()
        {
            // looks like the static extension is called before the ErrorInfo.Mode has trickled down.
            // fixing this can be slightly messy
            var groupBox = this.window.FindGroupBox(NoTranslationsWithReturnErrorInfoId);

            this.window.FindRadioButton("sv").Click();
            Assert.AreEqual("!MissingKey!", groupBox.FindTextBlock(MissingKeyTextBlockId).Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.FindTextBlock(NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindTextBlock(BadFromatTextBlockId).Text);

            this.window.FindRadioButton("en").Click();
            Assert.AreEqual("!MissingKey!", groupBox.FindTextBlock(MissingKeyTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.FindTextBlock(SwedishAndNeutralTextBlockId).Text);
            Assert.AreEqual("_So neutral_", groupBox.FindTextBlock(NeutralOnlyTextBlockId).Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindTextBlock(BadFromatTextBlockId).Text);
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