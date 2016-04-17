namespace Gu.Localization
{
    using System;
    using System.Globalization;
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
                throw new InvalidOperationException("T must be an enum when used in EnumTranslation<T>. Can't constrain it better this.");
            }
        }

        /// <summary> Create a translation for <paramref name="member"/> </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <paramref name="member"/></param>
        /// <param name="member"> The member to translate</param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns> A <see cref="Translation"/></returns>
        public static Translation Create(ResourceManager resourceManager, T member, ErrorHandling errorHandling = ErrorHandling.Default)
        {
            return Translation.GetOrCreate(resourceManager, member.ToString(CultureInfo.InvariantCulture), errorHandling);
        }
    }
}