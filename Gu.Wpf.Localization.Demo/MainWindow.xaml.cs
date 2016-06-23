namespace Gu.Wpf.Localization.Demo
{
    using System.Globalization;
    using System.Windows;

    using Gu.Localization;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Translator.EffectiveCultureChanged += (_, __) => this.LanguagesComboBox.SelectedItem = Translator.EffectiveCulture;
        }

        private void OnLanguagesComboBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            Translator.CurrentCulture = this.LanguagesComboBox.SelectedItem as CultureInfo;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.LanguagesComboBox.SelectedItem = Translator.EffectiveCulture;
        }
    }
}
