using System.Windows;

namespace Gu.Wpf.Localization.Demo.WithResources
{
    using System.Globalization;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TranslationManager.Instance.CurrentLanguage = CultureInfo.GetCultureInfo("sv");
        }
    }
}
