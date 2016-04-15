namespace Gu.Localization.Errors
{
    using System.CodeDom.Compiler;

    /// <summary>A base class for translation error.</summary>
    public abstract class TranslationError
    {
        protected TranslationError(string key)
        {
            this.Key = key;
        }

        /// <summary>Gets the key.</summary>
        public string Key { get; }

        internal abstract void WriteTo(IndentedTextWriter writer);
    }
}