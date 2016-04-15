namespace Gu.Localization.Errors
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Resources;

    public class ParameterCountMismatch : TranslationError
    {
        public ParameterCountMismatch(ResourceManager resourceManager, string key, IReadOnlyDictionary<CultureInfo, string> missingCultures)
            : base(resourceManager, key)
        {
            this.MissingCultures = missingCultures;
        }

        public IReadOnlyDictionary<CultureInfo, string> MissingCultures { get; }
    }
}