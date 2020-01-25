namespace Gu.Wpf.Localization.Demo
{
    using System.Globalization;
    using System.Threading;
    using System.Windows;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e is { Args: { Length: 1 } args })
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(args[0]);
            }

            base.OnStartup(e);
        }
    }
}
