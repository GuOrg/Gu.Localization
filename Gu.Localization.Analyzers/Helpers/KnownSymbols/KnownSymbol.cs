// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace Gu.Localization.Analyzers
{
    internal static class KnownSymbol
    {
        internal static readonly QualifiedType Void = Create("System.Void");
        internal static readonly QualifiedType Object = Create("System.Object", "object");
        internal static readonly QualifiedType Boolean = Create("System.Boolean", "bool");
        internal static readonly QualifiedType Int32 = Create("System.Int32", "int");
        internal static readonly QualifiedType Int64 = Create("System.Int64", "long");
        internal static readonly StringType String = new StringType();
        internal static readonly TranslatorType Translator = new TranslatorType();

        private static QualifiedType Create(string qualifiedName, string alias = null)
        {
            return new QualifiedType(qualifiedName, alias);
        }
    }
}
