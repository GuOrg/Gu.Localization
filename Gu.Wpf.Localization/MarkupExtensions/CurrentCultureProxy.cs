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

#pragma warning disable GU0073 // Member of non-public type should be internal. Must be public for binding to work.
        public CultureInfo Value => Translator.CurrentCulture;
#pragma warning restore GU0073 // Member of non-public type should be internal.

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
