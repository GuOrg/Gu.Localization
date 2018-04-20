namespace Gu.Localization.Analyzers
{
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

        public static bool IsResourceManager(ExpressionSyntax expression, out ExpressionSyntax resources)
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
    }
}
