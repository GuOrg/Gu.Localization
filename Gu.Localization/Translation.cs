namespace Gu.Localization
{
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    /// <inheritdoc />
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public partial class Translation : ITranslation
    {
        private static readonly PropertyChangedEventArgs TranslatedPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Translated));
        private static readonly ConcurrentDictionary<ResourceManagerAndKey, Translation> Cache = new ConcurrentDictionary<ResourceManagerAndKey, Translation>();

        private readonly string key;
        private readonly ErrorHandling errorHandling;
        private readonly ResourceManager resourceManager;
        private readonly CachedTranslation cachedTranslation;

        static Translation()
        {
            Translator.CurrentCultureChanged += (_, c) =>
                {
                    foreach (var translation in Cache.Values)
                    {
                        translation.OnCurrentCultureChanged(c);
                    }
                };
        }

        private Translation(ResourceManager resourceManager, string key, ErrorHandling errorHandling = ErrorHandling.Default)
        {
            this.resourceManager = resourceManager;
            this.key = key;
            this.errorHandling = errorHandling;
            this.cachedTranslation = new CachedTranslation(this);
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public string Translated => this.cachedTranslation.Value;

        /// <summary>
        /// Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey))
        /// </summary>
        /// <param name="resourceManager">
        /// The resourcemanager with the key
        /// </param>
        /// <param name="key">
        /// The key to translate
        /// </param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns>A <see cref="Translation"/> that notifies when <see cref="Translator.CurrentCulture"/> changes</returns>
        public static Translation GetOrCreate(ResourceManager resourceManager, string key, ErrorHandling errorHandling = ErrorHandling.Default)
        {
            Ensure.NotNull(resourceManager, nameof(resourceManager));
            Ensure.NotNull(key, nameof(key));

            var rmk = new ResourceManagerAndKey(resourceManager, key, errorHandling);
            return Cache.GetOrAdd(rmk, x => new Translation(x.ResourceManager, x.Key, errorHandling));
        }

        /// <summary>
        /// Calls <see cref="Translator.Translate(ResourceManager, string, CultureInfo, ErrorHandling)"/> with the key.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="errorHandlingStrategy">Specifiec how errors are handled</param>
        /// <returns>The translated string.</returns>
        public string Translate(CultureInfo culture, ErrorHandling errorHandlingStrategy = ErrorHandling.Default)
        {
            return Translator.Translate(this.resourceManager, this.key, culture, errorHandlingStrategy);
        }

        /// <summary> Use this to raise propertychanged</summary>
        /// <param name="propertyName">The name of the property</param>
        // ReSharper disable once UnusedMember.Global
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary> Called when <see cref="Translator.CurrentCulture"/> changes</summary>
        /// <param name="culture">The new culture</param>
        protected virtual void OnCurrentCultureChanged(CultureInfo culture)
        {
            if (this.cachedTranslation.TryUpdate(culture))
            {
                this.PropertyChanged?.Invoke(this, TranslatedPropertyChangedEventArgs);
            }
        }

        private class CachedTranslation
        {
            private readonly Translation translation;
            private readonly object gate = new object();
            private CultureInfo culture;
            private string value;

            public CachedTranslation(Translation translation)
            {
                this.translation = translation;
            }

            public string Value
            {
                get
                {
                    if (this.culture == null)
                    {
                        this.TryUpdate(Translator.CurrentCulture);
                    }

                    return this.value;
                }
            }

            public bool TryUpdate(CultureInfo cultureInfo)
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
                                this.translation.key,
                                cultureInfo,
                                this.translation.errorHandling);
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
