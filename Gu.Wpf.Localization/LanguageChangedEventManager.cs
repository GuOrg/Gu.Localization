namespace Gu.Wpf.Localization
{
    using System;
    using System.Windows;

    public class LanguageChangedEventManager : WeakEventManager
    {
        private static LanguageChangedEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(LanguageChangedEventManager);
                var manager = (LanguageChangedEventManager)GetCurrentManager(managerType);
                if (manager == null)
                {
                    manager = new LanguageChangedEventManager();
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }
      
        public static void AddListener(TranslationManager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }
        
        public static void RemoveListener(TranslationManager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }
        
        protected override void StartListening(object source)
        {
            var manager = (TranslationManager)source;
            manager.LanguageChanged += this.OnLanguageChanged;
        }
        
        protected override void StopListening(object source)
        {
            var manager = (TranslationManager)source;
            manager.LanguageChanged -= this.OnLanguageChanged;
        }
        
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            this.DeliverEvent(sender, e);
        }
    }
}