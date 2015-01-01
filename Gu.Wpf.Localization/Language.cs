namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Localization;
    using Gu.Wpf.Localization.Annotations;

    public class Language : INotifyPropertyChanged, IDisposable
    {
        private readonly CultureInfo _culture;

        private bool _isSelected;
        private bool _disposed = false;

        public Language(CultureInfo culture)
        {
            _culture = culture;
            Translator.LanguageChanged += OnLanguageChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsSelected
        {
            get
            {
                return Translator.CurrentCulture.TwoLetterISOLanguageName == _culture.TwoLetterISOLanguageName;
            }
            set
            {
                if (value.Equals(_isSelected))
                {
                    return;
                }
                if (value)
                {
                    Translator.CurrentCulture = Translator.AllCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == _culture.TwoLetterISOLanguageName);
                }
                OnPropertyChanged();
            }
        }

        public CultureInfo Culture
        {
            get
            {
                return _culture;
            }
        }

        public string Name
        {
            get
            {
                return _culture.Name;
            }
        }

        public string EnglishName
        {
            get
            {
                return _culture.EnglishName;
            }
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnLanguageChanged(object sender, CultureInfo e)
        {
            OnPropertyChanged("IsSelected");
        }
    }
}
