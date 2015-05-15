namespace Gu.Wpf.Localization.Internals
{
    using System.Text.RegularExpressions;

    internal class TypeNameAndKey
    {
        internal static bool TryParse(string member, out TypeNameAndKey result)
        {
            result = null;
            var match = Regex.Match(member, @"(?<prefix>\w+):(?<typeName>\w+)\.(?<key>\w+)");
            if (!match.Success)
            {
                return false;
            }
            var key = match.Groups["key"].Value;
            var prefix = match.Groups["prefix"].Value;
            var typeName = match.Groups["typeName"].Value;
            result = new TypeNameAndKey(prefix, typeName, key);
            return true;
        }

        public TypeNameAndKey(string prefix, string typeName, string key)
        {
            Prefix = prefix;
            TypeName = typeName;
            Key = key;
        }

        public string TypeName { get; private set; }

        public string Prefix { get; private set; }

        internal string QualifiedTypeName
        {
            get { return string.Format("{0}:{1}", Prefix, TypeName); }
        }

        internal string Key { get; private set; }
    }
}