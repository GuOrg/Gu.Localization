namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class AssemblyAndKey : IEquatable<AssemblyAndKey>
    {
        private static readonly ConcurrentDictionary<Assembly, ConcurrentDictionary<string, AssemblyAndKey>> Singletons = new ConcurrentDictionary<Assembly, ConcurrentDictionary<string, AssemblyAndKey>>();

        private AssemblyAndKey(Assembly assembly, string key)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            Assembly = assembly;
            Key = key;
        }

        public Assembly Assembly { get; }

        public string Key { get; }

        public static AssemblyAndKey GetOrCreate(Expression<Func<string>> key)
        {
            if (!ExpressionHelper.IsResourceKey(key))
            {
                throw new ArgumentException("Key is not a valid resource key. Expecting a key like: () => Properties.Resources.YourKey");
            }
            var resourceKey = ExpressionHelper.GetResourceKey(key);
           var  assembly = ExpressionHelper.GetRootType(key)
                                        .Assembly;
            return GetOrCreate(assembly, resourceKey);
        }

        public static AssemblyAndKey GetOrCreate(Assembly assembly, string key)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var assemblyAndKeys = Singletons.GetOrAdd(assembly, a => new ConcurrentDictionary<string, AssemblyAndKey>());
            return assemblyAndKeys.GetOrAdd(key, _ => new AssemblyAndKey(assembly, key));
        }

        public static bool operator ==(AssemblyAndKey left, AssemblyAndKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AssemblyAndKey left, AssemblyAndKey right)
        {
            return !Equals(left, right);
        }

        public bool Equals(AssemblyAndKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Assembly.Equals(other.Assembly) && string.Equals(Key, other.Key);
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
            return Equals((AssemblyAndKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Assembly.FullName.GetHashCode() * 397) ^ Key.GetHashCode();
            }
        }
    }
}
