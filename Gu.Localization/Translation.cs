namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Runtime.CompilerServices;

    /// <inheritdoc />
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public partial class Translation : ITranslation
    {
        private static readonly PropertyChangedEventArgs TranslatedPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Translated));
        private static readonly ConcurrentDictionary<ResourceManagerAndKey, ITranslation> Cache = new ConcurrentDictionary<ResourceManagerAndKey, ITranslation>();

        private readonly ResourceManager resourceManager;
        private readonly CachedTranslation cachedTranslation;

        static Translation()
        {
            Translator.CurrentCultureChanged += (_, c) =>
                {
                    foreach (var translation in Cache.Values.OfType<Translation>())
                    {
                        translation.OnCurrentCultureChanged(c.Culture);
                    }
                };
        }

        private Translation(ResourceManager resourceManager, string key, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            this.resourceManager = resourceManager;
            this.Key = key;
            this.ErrorHandling = errorHandling;
            this.cachedTranslation = new CachedTranslation(this);
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public string Translated => this.cachedTranslation.Value;

        /// <inheritdoc />
        public string Key { get; }

        /// <inheritdoc />
        public ErrorHandling ErrorHandling { get; }

        /// <summary>
        /// Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey))
        /// If <paramref name="resourceManager"/> contains the resource <paramref name="key"/> an <see cref="Translation"/> is returned.
        /// If not a static translation is returned if error handling is not throw.
        /// </summary>
        /// <param name="resourceManager">
        /// The <see cref="ResourceManager"/> with the key.
        /// </param>
        /// <param name="key">
        /// The key to translate.
        /// </param>
        /// <param name="errorHandlingStrategy">Specifies how errors are handled.</param>
        /// <returns>
        /// A <see cref="Translation"/> that notifies when <see cref="Translator.Culture"/> changes.
        /// </returns>
        public static ITranslation GetOrCreate(ResourceManager resourceManager, string key, ErrorHandling errorHandlingStrategy = ErrorHandling.Inherit)
        {
            Ensure.NotNull(resourceManager, nameof(resourceManager));
            Ensure.NotNull(key, nameof(key));

            errorHandlingStrategy = errorHandlingStrategy.Coerce();
            var rmk = new ResourceManagerAndKey(resourceManager, key, errorHandlingStrategy);
            return Cache.GetOrAdd(rmk, x => CreateTranslation(x.ResourceManager, x.Key, errorHandlingStrategy));
        }

        /// <inheritdoc />
        public string Translate(CultureInfo culture, ErrorHandling errorHandlingStrategy = ErrorHandling.Inherit)
        {
            return Translator.Translate(this.resourceManager, this.Key, culture, errorHandlingStrategy);
        }

        /// <summary> Use this to raise propertychanged.</summary>
        /// <param name="propertyName">The name of the property.</param>
        // ReSharper disable once UnusedMember.Global
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary> Called when <see cref="Translator.Culture"/> changes.</summary>
        /// <param name="culture">The new culture.</param>
        protected virtual void OnCurrentCultureChanged(CultureInfo culture)
        {
            if (this.cachedTranslation.TryUpdate(culture))
            {
                this.PropertyChanged?.Invoke(this, TranslatedPropertyChangedEventArgs);
            }
        }

        private static ITranslation CreateTranslation(ResourceManager resourceManager, string key, ErrorHandling errorHandling)
        {
            if (resourceManager.HasKey(key))
            {
                return new Translation(resourceManager, key, errorHandling);
            }

            if (errorHandling == ErrorHandling.Throw)
            {
                throw new ArgumentOutOfRangeException(nameof(key), $"The resourcemanager: {resourceManager.BaseName} does not have the key: {key}");
            }

            return new StaticTranslation(string.Format(CultureInfo.InvariantCulture, Properties.Resources.MissingKeyFormat, key), key, errorHandling);
        }

        private class CachedTranslation
        {
            private readonly Translation translation;
            private readonly object gate = new object();
            private CultureInfo culture;
            private string value;

            internal CachedTranslation(Translation translation)
            {
                this.translation = translation;
            }

            internal string Value
            {
                get
                {
                    if (this.culture is null)
                    {
                        this.TryUpdate(Translator.CurrentCulture);
                    }

                    return this.value;
                }
            }

            internal bool TryUpdate(CultureInfo cultureInfo)
            {
                if (!Culture.NameEquals(cultureInfo, this.culture))
                {
                    lock (this.gate)
                    {
                        if (!Culture.NameEquals(cultureInfo, this.culture))
                        {
                            this.culture = cultureInfo;
                            var newValue = Translator.Translate(
                                this.translation.resourceManager,
                                this.translation.Key,
                                cultureInfo,
                                this.translation.ErrorHandling);
                            var changed = newValue != this.value;
                            this.value = newValue;
                            return changed;
                        }

                        return false;
                    }
                }

                return false;
            }
        }
    }
}
