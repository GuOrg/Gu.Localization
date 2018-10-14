namespace Gu.Localization
{
    using System.Globalization;
    using System.Resources;

    /// <summary>
    /// Sample Translator{Properties.Resources}.Translate(nameof(Properties.Resources.SomeKey));.
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
        /// Call like this Translator&lt;Properties.Resources&gt;.Translate(nameof(Properties.Resources.SomeKey));.
        /// </summary>
        /// <param name="key">Path to the key. Must be included <typeparamref name="TResources"/>.</param>
        /// <returns>The key translated to the <see cref="Translator.Culture"/>.</returns>
        public static string Translate(string key)
        {
            return Translate(key, ErrorHandling.Inherit);
        }

        /// <summary>
        /// Call like this Translator{Properties.Resources}.Translate(nameof(Properties.Resources.SomeKey));.
        /// </summary>
        /// <param name="key">Path to the key. Must be included <typeparamref name="TResources"/>.</param>
        /// <param name="culture">The culture, if null CultureInfo.InvariantCulture is used.</param>
        /// <returns>The key translated to the <paramref name="culture"/>.</returns>
        public static string Translate(string key, CultureInfo culture)
        {
            return Translator.Translate(ResourceManager, key, culture, ErrorHandling.Inherit);
        }

        /// <summary>
        /// Call like this Translator&lt;Properties.Resources&gt;.Translate(nameof(Properties.Resources.SomeKey));.
        /// </summary>
        /// <param name="key">Path to the key. Must be included <typeparamref name="TResources"/>.</param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns>The key translated to the <see cref="Translator.Culture"/>.</returns>
        public static string Translate(string key, ErrorHandling errorHandling)
        {
            return Translator.Translate(ResourceManager, key, errorHandling);
        }

        /// <summary>
        /// Call like this Translator&lt;Properties.Resources&gt;.Translate(nameof(Properties.Resources.SomeKey));.
        /// </summary>
        /// <param name="key">Path to the key. Must be included <typeparamref name="TResources"/>.</param>
        /// <param name="culture">The culture, if null CultureInfo.InvariantCulture is used.</param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns>The key translated to the <paramref name="culture"/>.</returns>
        public static string Translate(string key, CultureInfo culture, ErrorHandling errorHandling)
        {
            return Translator.Translate(ResourceManager, key, culture, errorHandling);
        }

        /// <summary>
        /// Translator&lt;Properties.Resources&gt;.Translate(nameof(Properties.Resources.SomeKey__0__,));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <param name="key">The key in.</param>
        /// <param name="arg0">The argument will be used as string.Format(format, <paramref name="arg0"/>).</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <see cref="Translator.Culture"/>.</returns>
        public static string Translate<T>(string key, T arg0, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            return Translator.Translate(ResourceManager, key, Translator.Culture, arg0, errorHandling);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <param name="key">The key in.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="arg0">The argument will be used as string.Format(format, <paramref name="arg0"/>).</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <paramref name="culture"/>.</returns>
        public static string Translate<T>(string key, CultureInfo culture, T arg0, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            return Translator.Translate(ResourceManager, key, culture, arg0, errorHandling);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <typeparam name="T0">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <typeparam name="T1">The type of <paramref name="arg1"/> generic to avoid boxing.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="arg0">The argument will be used as first arguyment in string.Format(culture, format, <paramref name="arg0"/>, <paramref name="arg1"/>).</param>
        /// <param name="arg1">The argument will be used as second argument string.Format(culture, format, <paramref name="arg0"/>, <paramref name="arg1"/>).</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <see cref="Translator.Culture"/>.</returns>
        public static string Translate<T0, T1>(string key, T0 arg0, T1 arg1, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            return Translator.Translate(ResourceManager, key, Translator.Culture, arg0, arg1, errorHandling);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <typeparam name="T0">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <typeparam name="T1">The type of <paramref name="arg1"/> generic to avoid boxing.</typeparam>
        /// <param name="key">The key in <typeparamref name="TResources"/>.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="arg0">The argument will be used as first arguyment in string.Format(culture, format, <paramref name="arg0"/>, <paramref name="arg1"/>).</param>
        /// <param name="arg1">The argument will be used as second argument string.Format(culture, format, <paramref name="arg0"/>, <paramref name="arg1"/>).</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <paramref name="culture"/>.</returns>
        public static string Translate<T0, T1>(string key, CultureInfo culture, T0 arg0, T1 arg1, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            return Translator.Translate(ResourceManager, key, culture, arg0, arg1, errorHandling);
        }

        /// <summary>
        /// Create a <see cref="Translation"/> for <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns>A <see cref="Translation"/>.</returns>
        public static ITranslation GetOrCreateTranslation(string key, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            return Translation.GetOrCreate(ResourceManager, key, errorHandling);
        }
    }
}
