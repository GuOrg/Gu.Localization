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
            Translator.CurrentCultureChanged += (_, __) => this.LanguagesComboBox.SelectedItem = Translator.CurrentCulture;
        }

        private void OnLanguagesComboBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            Translator.Culture = this.LanguagesComboBox.SelectedItem as CultureInfo;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.LanguagesComboBox.SelectedItem = Translator.CurrentCulture;
        }
    }
}
