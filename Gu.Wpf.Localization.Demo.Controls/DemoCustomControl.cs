namespace Gu.Wpf.Localization.Demo.Controls
{
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;

    public class DemoCustomControl : Control
    {
        static DemoCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DemoCustomControl), new FrameworkPropertyMetadata(typeof(DemoCustomControl)));
        }

        protected override AutomationPeer OnCreateAutomationPeer() => new DemoCustomControlAutomationPeer(this);
    }
}
