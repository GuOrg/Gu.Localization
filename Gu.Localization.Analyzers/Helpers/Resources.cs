namespace Gu.Localization.Analyzers
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
                case MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax identifierName } memberAccess:
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

        internal static bool IsResourceManager(ExpressionSyntax expression, [NotNullWhen(true)] out ExpressionSyntax? resources)
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

        internal static bool IsResourceKey(ExpressionSyntax expression, [NotNullWhen(true)] out ExpressionSyntax? resources)
        {
            resources = null;
            if (expression is MemberAccessExpressionSyntax resourceManager &&
                IsResources(resourceManager.Expression))
            {
                resources = resourceManager.Expression;
            }

            return resources != null;
        }

        internal static bool TryGetKey(string text, [NotNullWhen(true)] out string? key)
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
                _ = builder.Insert(0, '_');
            }

            const int maxLength = 100;
            if (builder.Length > maxLength)
            {
                _ = builder.Remove(maxLength, builder.Length - maxLength);
            }

            key = builder.Return();
            if (key == "Resources")
            {
                key += "_";
            }

            return SyntaxFacts.IsValidIdentifier(key);
        }

        internal static IEnumerable<INamedTypeSymbol> LookupResourceTypes(this SemanticModel semanticModel, int position, string name, INamespaceSymbol? container = null)
        {
#pragma warning disable RS1024 // Compare symbols correctly
            return LookupResourceTypesCore().Distinct(NamedTypeSymbolComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly

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
