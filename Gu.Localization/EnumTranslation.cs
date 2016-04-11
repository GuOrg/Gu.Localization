namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class EnumTranslation<T> : ITranslation
        where T : struct, IComparable, IFormattable, IConvertible
    {
        private readonly Translation translation;
        private bool disposed = false;

        public EnumTranslation(ResourceManager resourceManager, T member)
        {
            this.translation = new Translation(resourceManager, member.ToString(CultureInfo.InvariantCulture));
            this.translation.PropertyChanged += this.TranslationOnPropertyChanged;
        }

        public EnumTranslation(ResourceManager resourceManager, Func<T> member, IObservable<object> trigger)
        {
            this.translation = new Translation(
                resourceManager,
                () => member().ToString(CultureInfo.InvariantCulture),
                trigger);
            this.translation.PropertyChanged += this.TranslationOnPropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Translated => this.translation.Translated;

        /// <summary>
        /// This will probably mostly be used in tests
        /// </summary>
        public IEnumerable<T> MissingTranslations
        {
            get
            {
                return Enum.GetValues(typeof(T))
                           .OfType<T>()
                           .Where(x => !this.translation.Translator.HasKey(x.ToString(CultureInfo.InvariantCulture)));
            }
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.translation.PropertyChanged -= this.TranslationOnPropertyChanged;
                this.translation.Dispose();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TranslationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(this.Translated))
            {
                this.OnPropertyChanged(nameof(this.Translated));
            }
        }
    }
}