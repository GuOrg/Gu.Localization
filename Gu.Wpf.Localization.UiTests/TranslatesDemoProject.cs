namespace Gu.Wpf.Localization.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public sealed class TranslatesDemoProject
    {
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(StartInfo.DemoProject.FileName);
        }

        [Test]
        public void EffectiveCulture()
        {
            using var application = Application.AttachOrLaunch(StartInfo.DemoProject);
            var window = application.MainWindow;
            window.FindRadioButton("en").IsChecked = true;
            Assert.AreEqual("en", window.FindTextBlock("CurrentCultureTextBlockId").Text);
            Assert.AreEqual("en", window.FindTextBlock("EffectiveCultureTextBlockId").Text);
            Assert.AreEqual("en", window.FindTextBox("BoundCurrentCultureTextBoxId").Text);
            Assert.AreEqual("en", window.FindTextBlock("BoundEffectiveCultureTextBlockId").Text);

            window.FindRadioButton("sv").IsChecked = true;
            Assert.AreEqual("sv", window.FindTextBlock("CurrentCultureTextBlockId").Text);
            Assert.AreEqual("sv", window.FindTextBlock("EffectiveCultureTextBlockId").Text);
            Assert.AreEqual("sv", window.FindTextBox("BoundCurrentCultureTextBoxId").Text);
            Assert.AreEqual("sv", window.FindTextBlock("BoundEffectiveCultureTextBlockId").Text);
        }

        [Test]
        public void VanillaXaml()
        {
            using var application = Application.AttachOrLaunch(StartInfo.DemoProject);
            var window = application.MainWindow;
            window.FindRadioButton("en").IsChecked = true;
            var groupBox = window.FindGroupBox("Vanilla xaml");
            Assert.AreEqual("English", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("English", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);

            window.FindRadioButton("sv").IsChecked = true;
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);

            window.FindRadioButton("en").IsChecked = true;
            Assert.AreEqual("English", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("English", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);
        }

        [Test]
        public void VanillaXamlWithErrorHandling()
        {
            using var application = Application.AttachOrLaunch(StartInfo.DemoProject);
            var window = application.MainWindow;
            window.FindRadioButton("en").IsChecked = true;
            var groupBox = window.FindGroupBox("Vanilla xaml with ReturnErrorInfo");
            Assert.AreEqual("English", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("_So neutral_", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("English", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);

            window.FindRadioButton("sv").IsChecked = true;
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);

            window.FindRadioButton("en").IsChecked = true;
            Assert.AreEqual("English", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual("_SwedishOnly_", groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("_So neutral_", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("English", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);
        }

        [Test]
        public void NotInVisualTree()
        {
            using var application = Application.AttachOrLaunch(StartInfo.DemoProject);
            var window = application.MainWindow;
            window.FindRadioButton("en").IsChecked = true;
            var groupBox = window.FindGroupBox("NotInVisualTreeGroupId");
            var dataGrid = groupBox.FindDataGrid("DataGridId");

            window.FindRadioButton("sv").IsChecked = true;
            Assert.AreEqual("Svenska", dataGrid.ColumnHeaders[0].Text);

            window.FindRadioButton("en").IsChecked = true;
            Assert.AreEqual("English", dataGrid.ColumnHeaders[0].Text);
        }

        [Test]
        public void DataTemplate()
        {
            using var application = Application.AttachOrLaunch(StartInfo.DemoProject);
            var window = application.MainWindow;
            window.FindRadioButton("en").IsChecked = true;
            var groupBox = window.FindGroupBox("DataTemplateGroupId");

            Assert.AreEqual("English", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("English", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);

            window.FindRadioButton("sv").IsChecked = true;
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);

            window.FindRadioButton("en").IsChecked = true;
            Assert.AreEqual("English", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("English", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);
        }

        [Test]
        public void UserControlSameProject()
        {
            using var application = Application.AttachOrLaunch(StartInfo.DemoProject);
            var window = application.MainWindow;
            window.FindRadioButton("en").IsChecked = true;
            var groupBox = window.FindGroupBox("UserControlSameProjectGroupId");

            Assert.AreEqual("English", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("English", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);

            window.FindRadioButton("sv").IsChecked = true;
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);

            window.FindRadioButton("en").IsChecked = true;
            Assert.AreEqual("English", groupBox.FindTextBlock("TranslatedToAllTextBlockId").Text);
            Assert.AreEqual(string.Empty, groupBox.FindTextBlock("SwedishOnlyTextBlockId").Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("English", groupBox.FindTextBlock("EnumTranslatedToAllTextBlockId").Text);
        }

        [Test]
        public void UserControlOtherProject()
        {
            using var application = Application.AttachOrLaunch(StartInfo.DemoProject);
            var window = application.MainWindow;
            window.FindRadioButton("en").IsChecked = true;
            var groupBox = window.FindGroupBox("UserControl from other project");
            var textBlock = groupBox.FindTextBlock("KeyInControls");

            window.FindRadioButton("sv").IsChecked = true;
            Assert.AreEqual("Från controls", textBlock.Text);

            window.FindRadioButton("en").IsChecked = true;
            Assert.AreEqual("From Controls", textBlock.Text);
        }

        [Test]
        [Explicit(Todo.FixThisTest)]
        public void CustomControlOtherProject()
        {
            using var application = Application.AttachOrLaunch(StartInfo.DemoProject);
            var window = application.MainWindow;
            window.FindRadioButton("en").IsChecked = true;
            var groupBox = window.FindGroupBox("CustomControlOtherProjectGroupId");
            var textBlock = groupBox.FindTextBlock("KeyInControls");

            window.FindRadioButton("sv").IsChecked = true;
            Assert.AreEqual("Från controls", textBlock.Text);

            window.FindRadioButton("en").IsChecked = true;
            Assert.AreEqual("From Controls", textBlock.Text);
        }

        [Test]
        public void NoTranslations()
        {
            using var application = Application.AttachOrLaunch(StartInfo.DemoProject);
            var window = application.MainWindow;
            window.FindRadioButton("en").IsChecked = true;
            var groupBox = window.FindGroupBox("NoTranslationsGroupId");

            window.FindRadioButton("sv").IsChecked = true;
            Assert.AreEqual("!MissingKey!", groupBox.FindTextBlock("MissingKeyTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock("NeutralOnlyTextBlockId").Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindTextBlock("BadFormatTextBlockId").Text);

            window.FindRadioButton("en").IsChecked = true;
            Assert.AreEqual("!MissingKey!", groupBox.FindTextBlock("MissingKeyTextBlockId").Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("So neutral", groupBox.FindTextBlock("NeutralOnlyTextBlockId").Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindTextBlock("BadFormatTextBlockId").Text);
        }

        [Test]
        [Explicit(Todo.FixThisTest)]
        public void NoTranslationsWithReturnErrorInfo()
        {
            using var application = Application.AttachOrLaunch(StartInfo.DemoProject);
            var window = application.MainWindow;
            window.FindRadioButton("en").IsChecked = true;

            // looks like the static extension is called before the ErrorInfo.Mode has trickled down.
            // fixing this can be slightly messy
            var groupBox = window.FindGroupBox("NoTranslationsWithReturnErrorInfoId");

            window.FindRadioButton("sv").IsChecked = true;
            Assert.AreEqual("!MissingKey!", groupBox.FindTextBlock("MissingKeyTextBlockId").Text);
            Assert.AreEqual("Svenska", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("_So neutral_", groupBox.FindTextBlock("NeutralOnlyTextBlockId").Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindTextBlock("BadFormatTextBlockId").Text);

            window.FindRadioButton("en").IsChecked = true;
            Assert.AreEqual("!MissingKey!", groupBox.FindTextBlock("MissingKeyTextBlockId").Text);
            Assert.AreEqual("_So neutral_", groupBox.FindTextBlock("SwedishAndNeutralTextBlockId").Text);
            Assert.AreEqual("_So neutral_", groupBox.FindTextBlock("NeutralOnlyTextBlockId").Text);
            Assert.AreEqual("#BadFormat#", groupBox.FindTextBlock("BadFormatTextBlockId").Text);
        }
    }
}
