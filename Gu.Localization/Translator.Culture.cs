namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    public static partial class Translator
    {
        private static CultureInfo currentCulture;
        private static SortedSet<CultureInfo> allCultures = GetAllCultures();

        private static CultureInfo effectiveCulture;

        /// <summary>Notifies when the current language changes.</summary>
        public static event EventHandler<CultureInfo> EffectiveCultureChanged;

        /// <summary>For binding to static properties in XAML.</summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        /// <summary> Gets a list with all cultures found for the application </summary>
        public static IEnumerable<CultureInfo> Cultures => allCultures;

        /// <summary>
        /// Gets or sets the culture to translate to.
        /// If setting to null EffectiveCulture is set to <see cref="CultureInfo.CurrentCulture"/> if there is a translation for it in <see cref="Cultures"/>
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return currentCulture;
            }

            set
            {
                if (CultureInfoComparer.ByName.Equals(currentCulture, value))
                {
                    return;
                }

                if (value != null && !value.IsInvariant() && !ContainsCulture(value))
                {
                    if (!allCultures.Any(c => Culture.TwoLetterIsoLanguageNameEquals(c, value)))
                    {
                        var message = "Can only set culture to an existing culture.\r\n" +
                                      $"Check the property {nameof(Cultures)} for a list of valid cultures.";
                        throw new ArgumentException(message);
                    }
                }

                currentCulture = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CurrentCulture)));
                EffectiveCulture = GetEffectiveCulture(currentCulture);
            }
        }

        /// <summary>
        /// Gets the culture that is used when translating.
        /// Uses a fallback mechanism:
        /// 1) CurrentCulture if not null.
        /// 2) Any Culture in <see cref="Cultures"/> matching <see cref="CultureInfo.CurrentCulture"/> by name.
        /// 3) Any Culture in <see cref="Cultures"/> matching <see cref="CultureInfo.CurrentCulture"/> by name.
        /// 4) CultureInfo.InvariantCulture
        /// </summary>
        /// <returns>The effective culture.</returns>
        public static CultureInfo EffectiveCulture
        {
            get
            {
                return effectiveCulture ?? (effectiveCulture = GetEffectiveCulture(null));
            }

            private set
            {
                if (Culture.NameEquals(value, effectiveCulture))
                {
                    return;
                }

                effectiveCulture = value;
                OnCurrentCultureChanged(value);
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CurrentCulture)));
            }
        }

        /// <summary>Check if <see cref="Cultures"/> contains a culture matching <paramref name="culture"/>.</summary>
        /// <param name="culture">The culture to test.</param>
        /// <returns>True if <see cref="Cultures"/> contains a match for <paramref name="culture"/></returns>
        public static bool ContainsCulture(CultureInfo culture)
        {
            if (culture == null)
            {
                return false;
            }

            if (allCultures == null || allCultures.Count == 0)
            {
                return false;
            }

            return allCultures?.Contains(culture) == true ||
                   allCultures?.Any(c => Culture.TwoLetterIsoLanguageNameEquals(c, culture)) == true;
        }

        private static CultureInfo GetEffectiveCulture(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                return allCultures?.FirstOrDefault(c => Culture.NameEquals(c, CultureInfo.CurrentCulture)) ??
                       allCultures?.FirstOrDefault(c => Culture.TwoLetterIsoLanguageNameEquals(c, CultureInfo.CurrentCulture)) ??
                       CultureInfo.InvariantCulture;
            }

            return allCultures?.FirstOrDefault(c => Culture.NameEquals(c, cultureInfo)) ??
                   allCultures?.FirstOrDefault(c => Culture.TwoLetterIsoLanguageNameEquals(c, cultureInfo)) ??
                   CultureInfo.InvariantCulture;
        }

        private static void OnCurrentCultureChanged(CultureInfo e)
        {
            EffectiveCultureChanged?.Invoke(null, e);
        }

        private static SortedSet<CultureInfo> GetAllCultures()
        {
            Debug.WriteLine(resourceDirectory);
            return resourceDirectory?.Exists == true
                       ? new SortedSet<CultureInfo>(ResourceCultures.GetAllCultures(resourceDirectory), CultureInfoComparer.ByName)
                       : new SortedSet<CultureInfo>(CultureInfoComparer.ByName);
        }
    }
}