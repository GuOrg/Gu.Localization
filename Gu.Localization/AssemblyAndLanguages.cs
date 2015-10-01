namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    public class AssemblyAndLanguages : IEquatable<AssemblyAndLanguages>
    {
        public AssemblyAndLanguages(Assembly assembly, IReadOnlyList<CultureInfo> cultures, IReadOnlyList<FileInfo> resourceFiles)
        {
            Assembly = assembly;
            Cultures = cultures;
            ResourceFiles = resourceFiles;
        }
        public Assembly Assembly { get; }

        public IReadOnlyList<CultureInfo> Cultures { get; }

        public IReadOnlyList<FileInfo> ResourceFiles { get; }

        public static bool operator ==(AssemblyAndLanguages left, AssemblyAndLanguages right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AssemblyAndLanguages left, AssemblyAndLanguages right)
        {
            return !Equals(left, right);
        }

        public bool Equals(AssemblyAndLanguages other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return AssemblyComparer.Default.Equals(this.Assembly, other.Assembly);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((AssemblyAndLanguages)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return AssemblyComparer.Default.GetHashCode(Assembly);
            }
        }
    }
}
