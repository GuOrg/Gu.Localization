namespace Gu.Localization.Errors
{
    using System.Resources;

    /// <summary>A base class for translation error.</summary>
    public abstract class TranslationError
    {
        protected TranslationError(ResourceManager resourceManager, string key)
        {
            this.ResourceManager = resourceManager;
            this.Key = key;
        }

        /// <summary>Gets the <see cref="ResourceManager"/>.</summary>
        public ResourceManager ResourceManager { get; }

        /// <summary>Gets the key.</summary>
        public string Key { get; }
    }
}