namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Resources;

    public static class EnumTranslation<T>
        where T : struct, IComparable, IFormattable, IConvertible
    {
        public static Translation Create(ResourceManager resourceManager, T member)
        {
            return Translation.GetOrCreate(resourceManager, member.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// This will probably mostly be used in tests
        /// </summary>
        public static IReadOnlyList<T> MissingTranslations(ResourceManager resourceManager)
        {
            return Enum.GetNames(typeof(T))
                       .Where(x => !Translator.HasKey(resourceManager, x.ToString(CultureInfo.InvariantCulture)))
                       .Select(name => Enum.Parse(typeof(T), name))
                       .OfType<T>()
                       .ToArray();
        }
    }
}