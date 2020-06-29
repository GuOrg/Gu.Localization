﻿namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Methods for working with <see cref="CultureInfo"/>.
    /// </summary>
    public static partial class Translator
    {
        private static CultureInfo? culture;
        private static CultureInfo? effectiveCulture;

        /// <summary>For binding to static properties in XAML.</summary>
        public static event EventHandler<PropertyChangedEventArgs>? StaticPropertyChanged;

        /// <summary>Notifies when the current language changes.</summary>
        public static event EventHandler<CultureChangedEventArgs>? CurrentCultureChanged;

        /// <summary> Gets a set with all cultures found for the application. </summary>
        public static ObservableSortedSet<CultureInfo> Cultures { get; } = new ObservableSortedSet<CultureInfo>(GetAllCultures(), CultureInfoComparer.ByName);

        /// <summary>
        /// Gets or sets the culture to translate to.
        /// If setting to null CurrentCulture is set to <see cref="CultureInfo.CurrentCulture"/> if there is a translation for it in <see cref="Cultures"/>.
        /// </summary>
        public static CultureInfo? Culture
        {
            get => culture;

            set
            {
                if (CultureInfoComparer.ByName.Equals(culture, value))
                {
                    return;
                }

                if (value != null &&
                    !value.IsInvariant() &&
                    !ContainsCulture(value) &&
                    !Cultures.Any(c => Localization.Culture.TwoLetterIsoLanguageNameEquals(c, value)))
                {
                    var message = "Can only set culture to an existing culture.\r\n" +
                                  $"Check the property {nameof(Cultures)} for a list of valid cultures.";
                    throw new ArgumentException(message);
                }

                culture = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Culture)));
                CurrentCulture = GetEffectiveCulture(culture);
            }
        }

        /// <summary>
        /// Gets the culture that is used when translating.
        /// Uses a fallback mechanism:
        /// 1) Culture if not null.
        /// 2) Any Culture in <see cref="Cultures"/> matching <see cref="CultureInfo.CurrentCulture"/> by name.
        /// 3) Any Culture in <see cref="Cultures"/> matching <see cref="CultureInfo.CurrentCulture"/> by name.
        /// 4) CultureInfo.InvariantCulture.
        /// </summary>
        /// <returns>The effective culture.</returns>
        public static CultureInfo CurrentCulture
        {
            get => effectiveCulture ??= GetEffectiveCulture(null);

            private set
            {
                if (effectiveCulture is { } &&
                    Localization.Culture.NameEquals(value, effectiveCulture))
                {
                    return;
                }

                effectiveCulture = value;
                OnCurrentCultureChanged(value);
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CurrentCulture)));
            }
        }

        /// <summary>Check if <see cref="Cultures"/> contains a culture matching <paramref name="language"/>.</summary>
        /// <param name="language">The culture to test.</param>
        /// <returns>True if <see cref="Cultures"/> contains a match for <paramref name="language"/>.</returns>
        public static bool ContainsCulture(CultureInfo? language)
        {
            if (language is null)
            {
                return false;
            }

            if (Cultures is null || Cultures.Count == 0)
            {
                return false;
            }

            return Cultures?.Contains(language) == true ||
                   Cultures?.Any(c => Localization.Culture.TwoLetterIsoLanguageNameEquals(c, language)) == true;
        }

        private static CultureInfo GetEffectiveCulture(CultureInfo? cultureInfo)
        {
            if (cultureInfo is null)
            {
                return Cultures?.FirstOrDefault(c => Localization.Culture.NameEquals(c, CultureInfo.CurrentCulture)) ??
                       Cultures?.FirstOrDefault(c => Localization.Culture.TwoLetterIsoLanguageNameEquals(c, CultureInfo.CurrentCulture)) ??
                       CultureInfo.InvariantCulture;
            }

            return Cultures?.FirstOrDefault(c => Localization.Culture.NameEquals(c, cultureInfo)) ??
                   Cultures?.FirstOrDefault(c => Localization.Culture.TwoLetterIsoLanguageNameEquals(c, cultureInfo)) ??
                   CultureInfo.InvariantCulture;
        }

        private static void OnCurrentCultureChanged(CultureInfo e)
        {
            CurrentCultureChanged?.Invoke(null, new CultureChangedEventArgs(e));
        }

        private static IEnumerable<CultureInfo> GetAllCultures()
        {
            Debug.WriteLine(resourceDirectory);
            return resourceDirectory?.Exists == true
                       ? ResourceCultures.GetAllCultures(resourceDirectory)
                       : Enumerable.Empty<CultureInfo>();
        }
    }
}
