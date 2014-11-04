namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    public static class DesignMode
    {
        public static bool? OverrideIsInDesignMode = null; // Hacking it ugly like this to be able to test
        private static readonly ResxTranslationProvider Provider = new ResxTranslationProvider(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic));
        private static readonly DependencyObject DependencyObject = new DependencyObject();
        
        /// <summary>
        /// Check if is in desigtime mode
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                if (OverrideIsInDesignMode.HasValue)
                {
                    return OverrideIsInDesignMode.Value;
                }
                return DesignerProperties.GetIsInDesignMode(DependencyObject);
            }
        }

        public static void AssertTranslation(IServiceProvider serviceProvider, string key)
        {
            if (!IsInDesignMode)
            {
                return;
            }
            var provideValueTarget = serviceProvider.ProvideValueTarget();
            if (provideValueTarget == null)
            {
                throw new ArgumentException("provideValueTarget == null");
            }
            if (!Provider.HasKey(key, null))
            {
                var dependencyProperty = (DependencyProperty)provideValueTarget.TargetProperty;
                var dependencyObject = (DependencyObject)provideValueTarget.TargetObject;
                dependencyObject.SetValue(dependencyProperty, string.Format(Properties.Resources.MissingKeyFormat, key));
                throw new ArgumentException(string.Format("No translation for {0}", key));
            }
        }
    }
}
