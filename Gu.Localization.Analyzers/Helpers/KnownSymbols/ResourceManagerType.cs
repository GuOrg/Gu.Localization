namespace Gu.Localization.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class ResourceManagerType : QualifiedType
    {
        internal ResourceManagerType()
            : base("System.Resources.ResourceManager")
        {
            this.GetObject = new QualifiedMethod(this, nameof(this.GetObject));
            this.GetString = new QualifiedMethod(this, nameof(this.GetString));
        }

        internal QualifiedMethod GetObject { get; }

        internal QualifiedMethod GetString { get; }
    }
}
