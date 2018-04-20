namespace Gu.Localization.Analyzers.Tests.Helpers
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class SolutionExt
    {
        internal static Document FindDocument(this Solution sln, string name)
        {
            foreach (var project in sln.Projects)
            {
                foreach (var document in project.Documents)
                {
                    if (document.Name == name)
                    {
                        return document;
                    }
                }
            }

            throw new InvalidOperationException($"Did not find a document with name {name}");
        }

        internal static TextDocument FindAdditionalDocument(this Solution sln, string name)
        {
            foreach (var project in sln.Projects)
            {
                foreach (var document in project.AdditionalDocuments)
                {
                    if (document.Name == name)
                    {
                        return document;
                    }
                }
            }

            throw new InvalidOperationException($"Did not find a document with name {name}");
        }
    }
}
