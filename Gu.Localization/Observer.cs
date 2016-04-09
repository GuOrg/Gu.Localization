namespace Gu.Localization
{
    using System;

    /// <summary>
    /// Don't want a reference to Rx in this lib. Doing it manually.
    /// </summary>
    internal class Observer : IObserver<object>
    {
        private readonly Action action;
        public Observer(Action action)
        {
            this.action = action;
        }
        public void OnNext(object value)
        {
            this.action();
        }
        public void OnError(Exception error)
        {
        }
        public void OnCompleted()
        {
        }
    }
}