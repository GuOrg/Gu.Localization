namespace Gu.Localization
{
    using System;
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
        private readonly CachedTranslation cachedTranslation;

        static Translation()
        {
            Translator.EffectiveCultureChanged += (_, c) =>
                {
                    foreach (var translation in Cache.Values)
                    {
                        translation.OnCurrentCultureChanged(c.Culture);
                    }
                };
        }

        private Translation(ResourceManager resourceManager, string key, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            if (!resourceManager.HasKey(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key), $"The resourcemanager: {resourceManager.BaseName} does not have the key: {key}");
            }

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
        /// </summary>
        /// <param name="resourceManager">
        /// The resourcemanager with the key
        /// </param>
        /// <param name="key">
        /// The key to translate
        /// </param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns>A <see cref="Translation"/> that notifies when <see cref="Translator.CurrentCulture"/> changes</returns>
        public static Translation GetOrCreate(ResourceManager resourceManager, string key, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            Ensure.NotNull(resourceManager, nameof(resourceManager));
            Ensure.NotNull(key, nameof(key));

            var rmk = new ResourceManagerAndKey(resourceManager, key, errorHandling);
            return Cache.GetOrAdd(rmk, x => new Translation(x.ResourceManager, x.Key, errorHandling));
        }

        /// <inheritdoc />
        public string Translate(CultureInfo culture, ErrorHandling errorHandlingStrategy = ErrorHandling.Inherit)
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
                        this.TryUpdate(Translator.EffectiveCulture);
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
