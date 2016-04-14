namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Resources;

    /// <summary> Helper for creating enum translations </summary>
    /// <typeparam name="T">An enum type</typeparam>
    public static class EnumTranslation<T>
        where T : struct, IComparable, IFormattable, IConvertible
    {
        static EnumTranslation()
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("T must be an enum when used in EnumTranslation<T>. Can't constrain it better than it is.");
            }
        }

        /// <summary> Create a translation for <paramref name="member"/> </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <paramref name="member"/></param>
        /// <param name="member"> The member to translate</param>
        /// <returns> A <see cref="Translation"/></returns>
        public static Translation Create(ResourceManager resourceManager, T member)
        {
            return Translation.GetOrCreate(resourceManager, member.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// This will probably mostly be used in tests
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <typeparamref name="T"/></param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <returns>A list with all members that does not have </returns>
        public static IReadOnlyList<T> MissingTranslations(ResourceManager resourceManager, CultureInfo culture)
        {
            return Enum.GetNames(typeof(T))
                       .Where(x => !Translator.HasKey(resourceManager, x.ToString(CultureInfo.InvariantCulture), culture))
                       .Select(name => Enum.Parse(typeof(T), name))
                       .OfType<T>()
                       .ToArray();
        }
    }
}