namespace Gu.Localization
{
    using System;
    using System.Globalization;
    using System.Resources;

    /// <summary> Helper for creating enum translations. </summary>
    /// <typeparam name="T">An enum type.</typeparam>
    [Obsolete("Use non-generic EnumTranslation")]
    public static class EnumTranslation<T>
        where T : System.Enum
    {
        /// <summary> Create a translation for <paramref name="member"/>. </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <paramref name="member"/>.</param>
        /// <param name="member"> The member to translate.</param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns> A <see cref="Translation"/>.</returns>
        [Obsolete("Use non-generic EnumTranslation")]
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static ITranslation Create(ResourceManager resourceManager, T member, ErrorHandling errorHandling = ErrorHandling.Inherit)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            return Translation.GetOrCreate(resourceManager, member.ToString(CultureInfo.InvariantCulture), errorHandling);
        }
    }
}
