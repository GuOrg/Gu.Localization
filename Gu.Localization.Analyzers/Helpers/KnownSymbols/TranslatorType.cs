namespace Gu.Localization.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class TranslatorType : QualifiedType
    {
        internal TranslatorType()
            : base("Gu.Localization.Translator")
        {
            this.Translate = new QualifiedMethod(this, nameof(this.Translate));
        }

        internal QualifiedMethod Translate { get; }
    }
}
