namespace Gu.Wpf.Localization.WithNeutralLanguage
{
    using System.Globalization;
    using System.Windows;
    using Gu.Localization;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Translator.Culture = CultureInfo.GetCultureInfo("en");
        }
    }
}
