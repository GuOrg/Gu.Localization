namespace Gu.Localization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// For enabling binding to Current language
    /// </summary>
    public class CultureProxy : INotifyPropertyChanged, IDisposable
    {
        private bool _disposed = false;

        public CultureProxy()
        {
            Translator.LanguageChanged += OnLanguageChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CultureInfo CurrentCulture
        {
            get { return Translator.CurrentCulture; }
            set { Translator.CurrentCulture = value; }
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Translator.LanguageChanged -= OnLanguageChanged;
            }

            // Free any unmanaged objects here. 
            _disposed = true;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnLanguageChanged(object sender, CultureInfo e)
        {
            OnPropertyChanged("CurrentCulture");
        }
    }
}
