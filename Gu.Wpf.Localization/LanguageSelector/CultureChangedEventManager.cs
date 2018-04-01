namespace Gu.Wpf.Localization
{
    using System;
    using System.Windows;
    using Gu.Localization;

    /// <summary>
    /// Manager for the DependencyObject.Loaded event.
    /// </summary>
    internal class CultureChangedEventManager : WeakEventManager
    {
        private CultureChangedEventManager()
        {
        }

        // get the event manager for the current thread
        private static CultureChangedEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(CultureChangedEventManager);
                var manager = (CultureChangedEventManager)GetCurrentManager(managerType);

                // at first use, create and register a new manager
                if (manager == null)
                {
                    manager = new CultureChangedEventManager();
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        internal static void UpdateHandler(EventHandler<CultureChangedEventArgs> handler)
        {
            var manager = CurrentManager;
            manager.ProtectedRemoveHandler(
                null,
                handler ?? throw new ArgumentNullException(nameof(handler)));

            manager.ProtectedAddHandler(
                null,
                handler);
        }

        /// <inheritdoc />
        protected override ListenerList NewListenerList() => new ListenerList<CultureChangedEventArgs>();

        /// <inheritdoc />
        protected override void StartListening(object source)
        {
            Translator.CurrentCultureChanged += this.OnCultureChanged;
        }

        /// <inheritdoc />
        protected override void StopListening(object source)
        {
            Translator.CurrentCultureChanged -= this.OnCultureChanged;
        }

        // event handler for Loaded event
        private void OnCultureChanged(object sender, CultureChangedEventArgs args)
        {
            this.DeliverEvent(sender, args);
        }
    }
}
