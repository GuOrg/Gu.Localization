namespace Gu.Localization
{
    using System;

    /// <summary>
    /// Don't want a reference to Rx in this lib. Doing it manually.
    /// </summary>
    internal class Observer : IObserver<object>
    {
        private readonly Action _action;
        public Observer(Action action)
        {
            _action = action;
        }
        public void OnNext(object value)
        {
            _action();
        }
        public void OnError(Exception error)
        {
        }
        public void OnCompleted()
        {
        }
    }
}