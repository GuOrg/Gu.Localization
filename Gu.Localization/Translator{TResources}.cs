namespace Gu.Localization
{
    using System.Globalization;
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
            return Translate(key, ErrorHandling.Default);
        }

        /// <summary>
        /// Call like this Translator{Properties.Resources}.Translate(nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="key">Path to the key. Must be included <typeparamref name="TResources"/>.</param>
        /// <param name="culture">The culture, if null CultureInfo.InvariantCulture is used</param>
        /// <returns>The key translated to the <paramref name="culture"/></returns>
        public static string Translate(string key, CultureInfo culture)
        {
            return Translator.Translate(ResourceManager, key, culture, ErrorHandling.Default);
        }

        /// <summary>
        /// Call like this Translator{Properties.Resources}.Translate(nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="key">Path to the key. Must be included <typeparamref name="TResources"/>.</param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns>The key translated to the <see cref="Translator.CurrentCulture"/></returns>
        public static string Translate(string key, ErrorHandling errorHandling)
        {
            return Translator.Translate(ResourceManager, key, errorHandling);
        }

        /// <summary>
        /// Call like this Translator{Properties.Resources}.Translate(nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="key">Path to the key. Must be included <typeparamref name="TResources"/>.</param>
        /// <param name="culture">The culture, if null CultureInfo.InvariantCulture is used</param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns>The key translated to the <paramref name="culture"/></returns>
        public static string Translate(string key, CultureInfo culture, ErrorHandling errorHandling)
        {
            return Translator.Translate(ResourceManager, key, culture, errorHandling);
        }

        /// <summary>
        /// Create a <see cref="Translation"/> for <paramref name="key"/>
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns>A <see cref="Translation"/></returns>
        public static Translation GetOrCreateTranslation(string key, ErrorHandling errorHandling = ErrorHandling.Default)
        {
            return Translation.GetOrCreate(ResourceManager, key, errorHandling);
        }
    }
}
