using System;
using System.Collections.Generic;

namespace Toggles
{
    public struct NewToggleValue
    {
        public Type SourceType { get; set; }
        public string Key { get; set; }
        public bool Active { get; set; }
    }

    public interface IToggleSource : IObservable<NewToggleValue>
    {
        void Start();
        void Stop();
    }
}
