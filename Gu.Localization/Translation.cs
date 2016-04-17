namespace Gu.Localization
{
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Resources;
    using System.Runtime.CompilerServices;

    /// <inheritdoc />
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public partial class Translation : ITranslation
    {
        private static readonly PropertyChangedEventArgs TranslatedPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Translated));
        private static readonly ConcurrentDictionary<ResourceManagerAndKey, Translation> Cache = new ConcurrentDictionary<ResourceManagerAndKey, Translation>();

        private readonly string key;
        private readonly ResourceManager resourceManager;

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

        private Translation(ResourceManager resourceManager, string key)
        {
            this.resourceManager = resourceManager;
            this.key = key;
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public string Translated => Translator.Translate(this.resourceManager, this.key, ErrorHandling.ReturnInfo);

        /// <summary>
        /// Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey))
        /// </summary>
        /// <param name="resourceManager">
        /// The resourcemanager with the key
        /// </param>
        /// <param name="key">
        /// The key to translate
        /// </param>
        /// <returns>A <see cref="Translation"/> that notifies when <see cref="Translator.CurrentCulture"/> changes</returns>
        public static Translation GetOrCreate(ResourceManager resourceManager, string key)
        {
            Ensure.NotNull(resourceManager, nameof(resourceManager));
            Ensure.NotNull(key, nameof(key));

            var rmk = new ResourceManagerAndKey(resourceManager, key);
            return Cache.GetOrAdd(rmk, x => new Translation(x.ResourceManager, x.Key));
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
            this.PropertyChanged?.Invoke(this, TranslatedPropertyChangedEventArgs);
        }
    }
}
