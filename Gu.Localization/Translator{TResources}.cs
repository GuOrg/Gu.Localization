namespace Gu.Localization
{
    using System.Resources;

    /// <summary>
    /// Sample Translator{Properties.Resources}.Translate(nameof(Properties.Resources.SomeKey));
    /// </summary>
    /// <typeparam name="TResources">
    /// Must be a generated resources file.
    /// </typeparam>
    public static class Translator<TResources>
    {
        // ReSharper disable once StaticMemberInGenericType, yes this is the idea
        private static readonly ResourceManager ResourceManager;

        static Translator()
        {
            ResourceManager = ResourceManagers.ForType(typeof(TResources));
        }

        /// <summary>
        /// Call like this Translator{Properties.Resources}.Translate(nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="key">Path to the key. Must be included <typeparamref name="TResources"/>.</param>
        /// <returns>The key translated to the <see cref="Translator.CurrentCulture"/></returns>
        public static string Translate(string key)
        {
            return Translator.Translate(ResourceManager, key);
        }

        public static Translation GetOrCreateTranslation(string key)
        {
            return Translation.GetOrCreate(ResourceManager, key);
        }
    }
}
