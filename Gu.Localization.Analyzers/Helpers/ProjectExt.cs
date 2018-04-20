namespace Gu.Localization.Analyzers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    internal static class ProjectExt
    {
        internal static IEnumerable<Project> ReferencedProjects(this Project project)
        {
            return project.ProjectReferences.Select(x => project.Solution.GetProject(x.ProjectId));
        }
    }
}
