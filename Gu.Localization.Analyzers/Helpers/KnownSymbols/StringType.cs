namespace Gu.Localization.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class StringType : QualifiedType
    {
        internal StringType()
            : base("System.String", "string")
        {
            this.Format = new QualifiedMethod(this, nameof(this.Format));
        }

        internal QualifiedMethod Format { get; }
    }
}
