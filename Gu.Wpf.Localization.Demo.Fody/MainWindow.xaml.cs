namespace Gu.Wpf.Localization.Demo.Fody
{
    using System.Globalization;
    using System.Windows;
    using Gu.Localization;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Translator.Culture = CultureInfo.GetCultureInfo("sv-SE");
        }
    }
}
