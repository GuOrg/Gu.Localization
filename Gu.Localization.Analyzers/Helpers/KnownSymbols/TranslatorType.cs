namespace Gu.Localization.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class TranslatorType : QualifiedType
    {
        internal readonly QualifiedMethod Translate;

        internal TranslatorType()
            : base("Gu.Localization.Translator")
        {
            this.Translate = new QualifiedMethod(this, nameof(this.Translate));
        }
    }
}
