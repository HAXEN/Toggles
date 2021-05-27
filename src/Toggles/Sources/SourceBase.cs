using System;
using System.Collections.Generic;

namespace Toggles.Sources
{
    public abstract class SourceBase : IToggleSource
    {
        protected readonly List<IObserver<NewToggleValue>> Observers = new();

        public IDisposable Subscribe(IObserver<NewToggleValue> observer)
        {
            if(Observers.Contains(observer) == false)
                Observers.Add(observer);

            return new UnSubscriber(Observers, observer);
        }

        protected void NotifyObservers(string key, bool isSet)
        {
            Observers.ForEach(x => x.OnNext(new NewToggleValue
            {
                SourceType = GetType(),
                Key = key,
                Active = isSet,
            }));
        }

        private class UnSubscriber : IDisposable
        {
            private readonly List<IObserver<NewToggleValue>> _observers;
            private readonly IObserver<NewToggleValue> _observer;

            public UnSubscriber(List<IObserver<NewToggleValue>> observers, IObserver<NewToggleValue> observer)
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

        public abstract void Start();
        public abstract void Stop();
    }
}
