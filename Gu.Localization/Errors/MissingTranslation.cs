namespace Gu.Localization.Errors
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Resources;

    public class MissingTranslation : TranslationError
    {
        public MissingTranslation(ResourceManager resourceManager, string key, IReadOnlyList<CultureInfo> missingCultures)
            : base(resourceManager, key)
        {
            this.MissingCultures = missingCultures;
        }

        public IReadOnlyList<CultureInfo> MissingCultures { get; }
    }
}
