namespace Gu.Localization.Analyzers.Tests.Helpers
{
    using System.IO;
    using NUnit.Framework;

    internal static class DirectoryInfoExt
    {
        internal static void CopyTo(this DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var dir in source.GetDirectories())
            {
                CopyTo(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (var file in source.GetFiles())
            {
#pragma warning disable GU0011 // Don't ignore the return value.
                file.CopyTo(Path.Combine(target.FullName, file.Name));
#pragma warning restore GU0011 // Don't ignore the return value.
            }
        }

        internal static FileInfo FindFile(this DirectoryInfo directory, string name)
        {
            var fileInfo = new FileInfo(Path.Combine(directory.FullName, name));
            Assert.True(fileInfo.Exists);
            return fileInfo;
        }
    }
}
