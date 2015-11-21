namespace Gu.Wpf.Localization.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Gu.Wpf.Localization.Designtime;

    internal class QualifiedNameAndKey
    {
        private static readonly Dictionary<string, QualifiedNameAndKey> Cache = new Dictionary<string, QualifiedNameAndKey>();
        public readonly string QualifiedName;
        public readonly string Key;

        private QualifiedNameAndKey(string qualifiedName, string key)
        {
            QualifiedName = qualifiedName;
            Key = key;
        }

        public static QualifiedNameAndKey Parse(string member)
        {
            QualifiedNameAndKey result;
            if (!Cache.TryGetValue(member, out result))
            {
                //var match = Regex.Match(member, @"(?<ns>\w+):(?<resources>\w+)\.(?<key>\w+)");
                var match = Regex.Match(member, @"(?<qn>\w+:\w+)\.(?<key>\w+)");
                if (!match.Success)
                {
                    if (Design.IsDesignMode)
                    {
                        throw new ArgumentException($"Expecting format 'p:Resources.Key' was:'{member}'");
                    }
                    result = new QualifiedNameAndKey(null, null);
                }
                else
                {
                    result = new QualifiedNameAndKey(match.Groups["qn"].Value, match.Groups["key"].Value);
                }

                Cache[member] = result;
            }
            return result;
        }
    }
}
