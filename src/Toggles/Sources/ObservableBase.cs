using System;
using System.Collections.Generic;

namespace Toggles.Sources
{
    public abstract class ObservableBase<T> : IObservable<T>
    {
        protected readonly List<IObserver<T>> Observers = new();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if(Observers.Contains(observer) == false)
                Observers.Add(observer);

            return new UnSubscriber(Observers, observer);
        }

        private class UnSubscriber : IDisposable
        {
            private readonly List<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;

            public UnSubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}