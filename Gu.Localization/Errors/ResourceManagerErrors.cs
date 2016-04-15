namespace Gu.Localization.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Resources;

    public class ResourceManagerErrors
    {
        public static ResourceManagerErrors For(ResourceManager resourceManager)
        {
            throw new NotImplementedException("message");
        }

        /// <summary>
        /// This will probably mostly be used in tests
        /// </summary>
        /// <typeparam name="T">An enum type</typeparam>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <typeparamref name="T"/></param>
        /// <returns>A list with all members that does not have </returns>
        public static IReadOnlyList<T> For<T>(ResourceManager resourceManager)
                    where T : struct, IComparable, IFormattable, IConvertible
        {
            foreach (var key in Enum.GetNames(typeof(T)))
            {
                
            }
            throw new NotImplementedException("message");
            //return Enum.GetNames(typeof(T))
            //           .Where(x => !Translator.HasKey(resourceManager, x.ToString(CultureInfo.InvariantCulture), culture))
            //           .Select(name => Enum.Parse(typeof(T), name))
            //           .OfType<T>()
            //           .ToArray();
        }
    }
}
