namespace Gu.Wpf.Localization.Internals
{
    using System.Text.RegularExpressions;

    internal class TypeNameAndKey
    {
        internal static bool TryParse(string member, out TypeNameAndKey result)
        {
            result = null;
            var match = Regex.Match(member, @"(?<ns>\w+):(?<resources>\w+)\.(?<key>\w+)");
            if (!match.Success)
            {
                return false;
            }
            var qualifiedTypeName = string.Format("{0}:{1}", match.Groups["ns"].Value, match.Groups["resources"].Value);
            var key = match.Groups["key"].Value;
            result = new TypeNameAndKey(qualifiedTypeName, key);
            return true;
        }

        public TypeNameAndKey(string qualifiedTypeName, string key)
        {
            QualifiedTypeName = qualifiedTypeName;
            Key = key;
        }

        internal string QualifiedTypeName { get; private set; }
        
        internal string Key { get; private set; }
    }
}