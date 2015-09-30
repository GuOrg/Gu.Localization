namespace Gu.Wpf.Localization.Designtime
{
    using System;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Windows.Markup;

    using Gu.Localization;

    public class TemplateTranslation : ITranslation
    {
        private readonly string _member;
        private readonly IXamlTypeResolver _xamlTypeResolver;

        public TemplateTranslation(string member, IXamlTypeResolver xamlTypeResolver)
        {
            _member = member;
            _xamlTypeResolver = xamlTypeResolver;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Translated
        {
            get
            {
                var match = Regex.Match(_member, @"(?<ns>\w+):(?<resources>\w+)\.(?<key>\w+)");
                if (!match.Success)
                {
                    if (DesignMode.IsDesignMode)
                    {
                        throw new ArgumentException($"Expecting format 'p:Resources.Key' was:'{_member}'");
                    }
                    return null;
                }
                var qualifiedTypeName = $"{match.Groups["ns"].Value}:{match.Groups["resources"].Value}";
                var type = _xamlTypeResolver.Resolve(qualifiedTypeName);
                var key = match.Groups["key"].Value;
                var assemblyAndKey = AssemblyAndKey.GetOrCreate(type.Assembly, key);
                return Translator.Translate(assemblyAndKey.Assembly, assemblyAndKey.Key);
            }
        }
    }
}
