namespace Gu.Wpf.Localization.Demo.WithResources
{
    using System.Runtime.CompilerServices;

    public static class AutomationIds
    {
        public static readonly string MainWindow = Create();

        public static readonly string NoTranslationsGroupId = Create();
        public static readonly string VanillaXamlGroupId = Create();
        public static readonly string DataTemplateGroupId = Create();
        public static readonly string NotInVisualTreeGroupId = Create();
        public static readonly string UserControlSameProjectGroupId = Create();
        public static readonly string UserControlOtherProjectGroupId = Create();
        public static readonly string CustomControlOtherProjectGroupId = Create();

        public static readonly string LanguagesComboBoxId = Create();
        public static readonly string DataGridId = Create();
        public static readonly string TranslatedToAllTextBlockId = Create();
        public static readonly string SwedishOnlyTextBlockId = Create();

        private static string Create([CallerMemberName] string name = null)
        {
            return name;
        }
    }
}