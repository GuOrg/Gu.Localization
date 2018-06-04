namespace Gu.Wpf.Localization
{
    using System.ComponentModel;
    using System.Globalization;
    using Gu.Localization;

    internal class CurrentCultureProxy : INotifyPropertyChanged
    {
        internal static readonly CurrentCultureProxy Instance = new CurrentCultureProxy();

        private CurrentCultureProxy()
        {
            Translator.CurrentCultureChanged += (_, __) => this.OnPropertyChanged(nameof(this.Value));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CultureInfo Value => Translator.CurrentCulture;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
