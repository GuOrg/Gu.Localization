﻿namespace Gu.Localization
{
    using System.Globalization;
    using System.Resources;

    /// <summary>
    /// Methods for working with format strings.
    /// </summary>
    public static partial class Translator
    {
        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/>.</param>
        /// <param name="arg0">The argument will be used as string.Format(format, <paramref name="arg0"/>).</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <see cref="Culture"/>.</returns>
        public static string? Translate<T>(ResourceManager resourceManager, string key, T arg0, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            return Translate(resourceManager, key, CurrentCulture, arg0, errorHandling);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/>.</param>
        /// <param name="language">The culture.</param>
        /// <param name="arg0">The argument will be used as string.Format(format, <paramref name="arg0"/>).</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <paramref name="language"/>.</returns>
        public static string? Translate<T>(ResourceManager resourceManager, string key, CultureInfo language, T arg0, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            if (!TryTranslateOrThrow(resourceManager, key, language, errorHandling, out var format))
            {
                return format;
            }

            if (ShouldThrow(errorHandling))
            {
                Validate.Format(format, arg0);
                return string.Format(language, format, arg0);
            }

            if (!Validate.IsValidFormat(format, arg0))
            {
                return string.Format(language, Properties.Resources.InvalidFormat, format, arg0);
            }

            try
            {
                return string.Format(language, format, arg0);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return string.Format(language, Properties.Resources.InvalidFormat, format, arg0);
            }
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <typeparam name="T0">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <typeparam name="T1">The type of <paramref name="arg1"/> generic to avoid boxing.</typeparam>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/>.</param>
        /// <param name="arg0">The argument will be used as first argument in string.Format(culture, format, <paramref name="arg0"/>, <paramref name="arg1"/>).</param>
        /// <param name="arg1">The argument will be used as second argument string.Format(culture, format, <paramref name="arg0"/>, <paramref name="arg1"/>).</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <see cref="Culture"/>.</returns>
        public static string? Translate<T0, T1>(ResourceManager resourceManager, string key, T0 arg0, T1 arg1, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            return Translate(resourceManager, key, CurrentCulture, arg0, arg1, errorHandling);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <typeparam name="T0">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <typeparam name="T1">The type of <paramref name="arg1"/> generic to avoid boxing.</typeparam>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/>.</param>
        /// <param name="language">The culture.</param>
        /// <param name="arg0">The argument will be used as first argument in string.Format(culture, format, <paramref name="arg0"/>, <paramref name="arg1"/>).</param>
        /// <param name="arg1">The argument will be used as second argument string.Format(culture, format, <paramref name="arg0"/>, <paramref name="arg1"/>).</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <paramref name="language"/>.</returns>
        public static string? Translate<T0, T1>(ResourceManager resourceManager, string key, CultureInfo language, T0 arg0, T1 arg1, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            if (!TryTranslateOrThrow(resourceManager, key, language, errorHandling, out var format))
            {
                return format;
            }

            if (ShouldThrow(errorHandling))
            {
                Validate.Format(format, arg0, arg1);
                return string.Format(language, format, arg0, arg1);
            }

            if (!Validate.IsValidFormat(format, arg0, arg1))
            {
                return string.Format(language, Properties.Resources.InvalidFormat, format, string.Join(", ", arg0, arg1));
            }

            try
            {
                return string.Format(language, format, arg0, arg1);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return string.Format(language, Properties.Resources.InvalidFormat, format, string.Join(", ", arg0, arg1));
            }
        }

        /////// <summary>
        /////// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /////// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /////// </summary>
        /////// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /////// <param name="key">The key in <paramref name="resourceManager"/></param>
        /////// <param name="errorHandling">Specifies how to handle errors.</param>
        /////// <param name="args">The arguments will be used as first argument in string.Format(culture, format, <paramref name="args"/>)</param>
        /////// <returns>The key translated to the <see cref="Culture"/></returns>
        ////public static string Translate(
        ////    ResourceManager resourceManager,
        ////    string key,
        ////    ErrorHandling errorHandling = ErrorHandling.Default,
        ////    params object[] args)
        ////{
        ////    return Translate(resourceManager, key, Culture, errorHandling, args);
        ////}

        /////// <summary>
        /////// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /////// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /////// </summary>
        /////// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /////// <param name="key">The key in <paramref name="resourceManager"/></param>
        /////// <param name="culture">The culture.</param>
        /////// <param name="errorHandling">Specifies how to handle errors.</param>
        /////// <param name="args">The arguments will be used as first argument in string.Format(culture, format, <paramref name="args"/>)</param>
        /////// <returns>The key translated to the <see cref="Culture"/></returns>
        ////public static string Translate(
        ////    ResourceManager resourceManager,
        ////    string key,
        ////    CultureInfo culture,
        ////    ErrorHandling errorHandling = ErrorHandling.Default,
        ////    params object[] args)
        ////{
        ////    string format;
        ////    if (!TryTranslateOrThrow(resourceManager, key, culture, errorHandling, out format))
        ////    {
        ////        return format;
        ////    }

        ////    if (ShouldThrow(errorHandling))
        ////    {
        ////        Validate.Format(format, args);
        ////        return string.Format(culture, format, args);
        ////    }

        ////    if (!Validate.IsValidFormat(format, args))
        ////    {
        ////        return string.Format(culture, Properties.Resources.InvalidFormat, format, string.Join(", ", args));
        ////    }

        ////    try
        ////    {
        ////        return string.Format(culture, format, args);
        ////    }
        ////    catch (Exception)
        ////    {
        ////        return string.Format(culture, Properties.Resources.InvalidFormat, format, string.Join(", ", args));
        ////    }
        ////}
    }
}
