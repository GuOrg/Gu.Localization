namespace Gu.Wpf.Localization
{
    using System.ComponentModel;
    using System.Globalization;
    using Gu.Localization;

    internal class EffectiveCultureProxy : INotifyPropertyChanged
    {
        internal static readonly EffectiveCultureProxy Instance = new EffectiveCultureProxy();

        private EffectiveCultureProxy()
        {
            Translator.EffectiveCultureChanged += (_, __) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Value)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CultureInfo Value => Translator.EffectiveCulture;
    }
}