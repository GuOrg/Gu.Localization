namespace Gu.Localization
{
    using System.Resources;

    /// <summary> Helper for creating enum translations. </summary>
    public static class EnumTranslation
    {
        /// <summary> Create a translation for <paramref name="member"/>. </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <paramref name="member"/>.</param>
        /// <param name="member"> The member to translate.</param>
        /// <param name="errorHandling">Specifies how errors are handled.</param>
        /// <returns> A <see cref="Translation"/>.</returns>
        public static ITranslation Create<T>(ResourceManager resourceManager, T member, ErrorHandling errorHandling = ErrorHandling.Inherit)
            where T : System.Enum
        {
            return Translation.GetOrCreate(resourceManager, member.ToString(), errorHandling);
        }
    }
}
