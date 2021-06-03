using System;

namespace Toggles
{
    public interface IInnerLiveSource<out T> : IObservable<T>
    {
        void Start();
        void Stop();
    }
}
