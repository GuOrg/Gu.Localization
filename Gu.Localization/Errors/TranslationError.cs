namespace Gu.Localization.Errors
{
    using System.CodeDom.Compiler;

    /// <summary>A base class for translation error.</summary>
    public abstract class TranslationError
    {
        /// <summary>Initializes a new instance of the <see cref="TranslationError"/> class.</summary>
        /// <param name="key">The key</param>
        protected TranslationError(string key)
        {
            this.Key = key;
        }

        public string Key { get; }

        /// <summary>Append self to <paramref name="writer"/>.</summary>
        /// <param name="writer">The writer</param>
        internal abstract void WriteTo(IndentedTextWriter writer);
    }
}