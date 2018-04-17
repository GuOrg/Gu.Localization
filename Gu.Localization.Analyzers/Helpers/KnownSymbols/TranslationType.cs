namespace Gu.Localization.Analyzers
{
    internal class TranslationType : QualifiedType
    {
        internal readonly QualifiedMethod GetOrCreate;

        internal TranslationType()
            : base("Gu.Localization.Translation")
        {
            this.GetOrCreate = new QualifiedMethod(this, nameof(this.GetOrCreate));
        }
    }
}