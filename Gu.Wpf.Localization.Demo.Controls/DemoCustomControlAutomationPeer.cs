namespace Gu.Wpf.Localization.Demo.Controls
{
    using System.Windows.Automation.Peers;

    public class DemoCustomControlAutomationPeer : FrameworkElementAutomationPeer
    {
        public DemoCustomControlAutomationPeer(DemoCustomControl owner) : base(owner)
        {
        }

        protected override string GetClassNameCore() => "DemoCustomControl";

        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Custom;
    }
}