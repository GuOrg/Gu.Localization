namespace Gu.Localization.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
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
