namespace Gu.Localization.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class TranslationType : QualifiedType
    {
        internal TranslationType()
            : base("Gu.Localization.Translation")
        {
            this.GetOrCreate = new QualifiedMethod(this, nameof(this.GetOrCreate));
        }

        internal QualifiedMethod GetOrCreate { get; }
    }
}
