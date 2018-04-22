namespace Gu.Localization.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class ResourceManagerType : QualifiedType
    {
        internal ResourceManagerType()
            : base("System.Resources.ResourceManager")
        {
            this.GetObject = new QualifiedMethod(this, nameof(this.GetObject));
        }

        internal QualifiedMethod GetObject { get; }
    }
}
