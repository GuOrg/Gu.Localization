#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable SA1401 // Fields must be private
namespace Gu.Wpf.Localization
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal class QualifiedNameAndKey
    {
        internal readonly string QualifiedName;

        internal readonly string Key;

        private static readonly Dictionary<string, QualifiedNameAndKey> Cache = new Dictionary<string, QualifiedNameAndKey>();

        private QualifiedNameAndKey(string qualifiedName, string key)
        {
            this.QualifiedName = qualifiedName;
            this.Key = key;
        }

        internal static QualifiedNameAndKey Parse(string member)
        {
            if (Cache.TryGetValue(member, out var result))
            {
                return result;
            }

            var match = Regex.Match(member, @"(?<qn>\w+:\w+)\.(?<key>\w+)");
            result = !match.Success
                         ? new QualifiedNameAndKey(null, $"Expecting format 'p:Resources.Key' was:'{member}'")
                         : new QualifiedNameAndKey(match.Groups["qn"].Value, match.Groups["key"].Value);

            Cache[member] = result;

            return result;
        }
    }
}
