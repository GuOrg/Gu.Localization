namespace Gu.Wpf.Localization
{
    using System.ComponentModel;
    using System.Globalization;

    public class CultureProxy : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CultureProxy()
        {
            Translator.LanguageChanged += (sender, info) => OnPropertyChanged("CurrentCulture");
        }

        public CultureInfo CurrentCulture
        {
            get { return Translator.CurrentCulture; }
            set { Translator.CurrentCulture = value; }
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
