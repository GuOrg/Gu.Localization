using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using Gu.Localization;
using Gu.Wpf.Localization.Internals;

namespace Gu.Wpf.Localization
{
    internal class DesigntimeTranslation : ITranslation
    {
        private readonly string _member;
        private readonly IServiceProvider _serviceProvider;
        private IXamlTypeResolver _xamlTypeResolver;

        public DesigntimeTranslation(string member, IServiceProvider serviceProvider)
        {
            _member = member;
            _serviceProvider = serviceProvider;
            _xamlTypeResolver = serviceProvider.GetXamlTypeResolver();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private IXamlTypeResolver XamlTypeResolver => _xamlTypeResolver ?? (_xamlTypeResolver = _serviceProvider.GetXamlTypeResolver());

        public string Translated
        {
            get
            {
                var match = Regex.Match(_member, @"(?<ns>\w+):(?<resources>\w+)\.(?<key>\w+)");
                if (!match.Success)
                {
                    if (DesignTime.IsDesignMode)
                    {
                        throw new ArgumentException($"Expecting format 'p:Resources.Key' was:'{_member}'");
                    }
                    return null;
                }
                var qualifiedTypeName = $"{match.Groups["ns"].Value}:{match.Groups["resources"].Value}";
                var type = XamlTypeResolver.Resolve(qualifiedTypeName);
                var key = match.Groups["key"].Value;
                var assemblyAndKey = AssemblyAndKey.GetOrCreate(type.Assembly, key);
                return Translator.Translate(assemblyAndKey.Assembly, assemblyAndKey.Key);
            }
        }
    }
}