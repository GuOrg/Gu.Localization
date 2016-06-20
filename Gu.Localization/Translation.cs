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
        private readonly ResourceManager resourceManager;
        private string translated;

        static Translation()
        {
            Translator.CurrentCultureChanged += (_, __) =>
                {
                    foreach (var translation in Cache)
                    {
                        translation.Value.OnCurrentCultureChanged();
                    }
                };
        }

        private Translation(ResourceManager resourceManager, string key, ErrorHandling errorHandling = ErrorHandling.Default)
        {
            this.resourceManager = resourceManager;
            this.Key = key;
            this.ErrorHandling = errorHandling;
            this.translated = Translator.Translate(resourceManager, key, errorHandling);
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets the key that is used by this translation.</summary>
        public string Key { get; }

        /// <summary>Gets the errorhandling mode used by this translation.</summary>
        public ErrorHandling ErrorHandling { get; }

        /// <inheritdoc />
        public string Translated
        {
            get
            {
                return this.translated;
            }

            private set
            {
                if (this.translated == value)
                {
                    return;
                }

                this.translated = value;
                this.PropertyChanged?.Invoke(this, TranslatedPropertyChangedEventArgs);
            }
        }

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
        /// Calls <see cref="Translator.Translate(System.Resources.ResourceManager, string, CultureInfo, Localization.ErrorHandling)"/> with the key.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="errorHandlingStrategy">Specifiec how errors are handled</param>
        /// <returns>The translated string.</returns>
        public string Translate(CultureInfo culture, ErrorHandling errorHandlingStrategy = ErrorHandling.Default)
        {
            return Translator.Translate(this.resourceManager, this.Key, culture, errorHandlingStrategy);
        }

        /// <summary> Use this to raise propertychanged</summary>
        /// <param name="propertyName">The name of the property</param>
        // ReSharper disable once UnusedMember.Global
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary> Called when <see cref="Translator.CurrentCulture"/> changes</summary>
        protected virtual void OnCurrentCultureChanged()
        {
            this.Translated = Translator.Translate(this.resourceManager, this.Key, this.ErrorHandling);
        }
    }
}
