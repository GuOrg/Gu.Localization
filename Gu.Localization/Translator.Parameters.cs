namespace Gu.Localization
{
    using System;
    using System.Globalization;
    using System.Resources;

    public static partial class Translator
    {
        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <param name="arg">The argument will be used as string.Format(format, <paramref name="arg"/>)</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        internal static string Translate(ResourceManager resourceManager, string key, object arg, ErrorHandling errorHandling = ErrorHandling.Default)
        {
            return Translate(resourceManager, key, CurrentCulture, arg, errorHandling);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <param name="culture">The culture.</param>
        /// <param name="arg">The argument will be used as string.Format(format, <paramref name="arg"/>)</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        internal static string Translate(ResourceManager resourceManager, string key, CultureInfo culture, object arg, ErrorHandling errorHandling = ErrorHandling.Default)
        {
            string format;
            if (!TryTranslateOrThrow(resourceManager, key, culture, errorHandling, out format))
            {
                return format;
            }

            if (ShouldThrow(errorHandling))
            {
                Validate.Format(format, arg);
                return string.Format(culture, format, arg);
            }

            try
            {
                return string.Format(format, arg);
            }
            catch (Exception)
            {
                return string.Format(culture, Properties.Resources.InvalidFormat, format, arg);
            }
        }
    }
}