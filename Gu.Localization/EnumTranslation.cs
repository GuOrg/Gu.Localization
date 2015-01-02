namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using Annotations;

    public class EnumTranslation<T> : ITranslation
        where T : struct, IComparable, IFormattable, IConvertible
    {
        private readonly Translation _translation;
        private bool _disposed = false;

        public EnumTranslation(ResourceManager resourceManager, T member)
        {
            _translation = new Translation(resourceManager, member.ToString(CultureInfo.InvariantCulture));
            _translation.PropertyChanged += TranslationOnPropertyChanged;
        }

        public EnumTranslation(ResourceManager resourceManager, Func<T> member, IObservable<object> trigger)
        {
            _translation = new Translation(
                resourceManager,
                () => member().ToString(CultureInfo.InvariantCulture),
                trigger);
            _translation.PropertyChanged += TranslationOnPropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Translated { get { return _translation.Translated; } }

        /// <summary>
        /// This will probably mostly be used in tests
        /// </summary>
        public IEnumerable<T> MissingTranslations
        {
            get
            {
                return Enum.GetValues(typeof(T))
                           .OfType<T>()
                           .Where(x => !_translation.Translator.HasKey(x.ToString(CultureInfo.InvariantCulture)));
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
                _translation.PropertyChanged -= TranslationOnPropertyChanged;
                _translation.Dispose();
                // Free any other managed objects here. 
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

        private void TranslationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == ExpressionHelper.PropertyName(() => _translation.Translated))
            {
                OnPropertyChanged(ExpressionHelper.PropertyName(() => Translated));
            }
        }
    }
}