namespace Gu.Localization.Analyzers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class Resources
    {
        internal static bool IsResources(this ExpressionSyntax candidate)
        {
            switch (candidate)
            {
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression is IdentifierNameSyntax identifierName:
                    if (identifierName.Identifier.ValueText == "Resources")
                    {
                        return true;
                    }

                    if (identifierName.Identifier.ValueText == "Properties")
                    {
                        return IsResources(memberAccess.Name);
                    }

                    break;
                case IdentifierNameSyntax identifierName:
                    return identifierName.Identifier.ValueText == "Resources";
            }

            return false;
        }

        internal static bool IsResourceManager(ExpressionSyntax expression, out ExpressionSyntax resources)
        {
            resources = null;
            if (expression is MemberAccessExpressionSyntax resourceManager &&
                resourceManager.Name.Identifier.ValueText == "ResourceManager" &&
                IsResources(resourceManager.Expression))
            {
                resources = resourceManager.Expression;
            }

            return resources != null;
        }

        internal static bool IsResourceKey(ExpressionSyntax expression, out ExpressionSyntax resources)
        {
            resources = null;
            if (expression is MemberAccessExpressionSyntax resourceManager &&
                IsResources(resourceManager.Expression))
            {
                resources = resourceManager.Expression;
            }

            return resources != null;
        }

        internal static bool TryGetKey(string text, out string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                key = null;
                return false;
            }

            var builder = StringBuilderPool.Borrow()
                                           .Append(Regex.Replace(text, "{(?<n>\\d+)}", x => $"__{x.Groups["n"].Value}__"))
                                           .Replace(" ", "_")
                                           .Replace("\r\n", "_n_")
                                           .Replace("\n", "_n_")
                                           .Replace(".", "_")
                                           .Replace(",", "_")
                                           .Replace(":", "_");

            if (char.IsDigit(builder[0]))
            {
                builder.Insert(0, '_').IgnoreReturnValue();
            }

            const int maxLength = 100;
            if (builder.Length > maxLength)
            {
                builder.Remove(maxLength, builder.Length - maxLength).IgnoreReturnValue();
            }

            key = builder.Return();
            return SyntaxFacts.IsValidIdentifier(key);
        }

        internal static IEnumerable<INamedTypeSymbol> LookupResourceTypes(this SemanticModel semanticModel, int position, string name, INamespaceSymbol container = null)
        {
            return LookupResourceTypesCore().Distinct();

            IEnumerable<INamedTypeSymbol> LookupResourceTypesCore()
            {
                var namespacesAndTypes = semanticModel.LookupNamespacesAndTypes(position, container);
                foreach (var candidate in namespacesAndTypes)
                {
                    if (candidate is INamedTypeSymbol namedType &&
                        namedType.MetadataName == name &&
                        namedType.TryFindProperty("ResourceManager", out _))
                    {
                        yield return namedType;
                    }
                }

                foreach (var candidate in namespacesAndTypes)
                {
                    if (candidate is INamespaceSymbol namespaceSymbol)
                    {
                        foreach (var resourceType in LookupResourceTypes(semanticModel, position, name, namespaceSymbol))
                        {
                            yield return resourceType;
                        }
                    }
                }
            }
        }
    }
}
