using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggles.Impl
{
    internal class ToggleManager : IToggle, IObserver<NewToggleValue>
    {
        private readonly IEnumerable<(int priority, IToggleSource instance)> _sources;
        private readonly Dictionary<string, (int priority, bool isOn)> _currents = new();

        public ToggleManager(IEnumerable<(int priority, IToggleSource instance)> sources = null)
        {
            _sources = sources ?? new (int priority, IToggleSource instance)[0];

            foreach (var source in _sources)
            {
                source.instance.Subscribe(this);
            }

            foreach (var source in _sources)
            {
                source.instance.Start();
            }
        }

        public bool IsOn(string key)
        {
            if (_currents.ContainsKey(key))
                return _currents[key].isOn;

            return false;
        }

        public IEnumerable<(int priority, Type sourceType, IToggleSource sourceInstance)> Sources =>
            _sources.Select(x => (x.priority, x.instance.GetType(), x.instance)).ToArray();

        public void OnCompleted()
        {
            
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(NewToggleValue value)
        {
            var priority = _sources.First(x => x.instance.GetType() == value.SourceType).priority;

            if (_currents.ContainsKey(value.Key) == false)
            {
                _currents[value.Key] = (priority, value.Active);
                return;
            }

            if(_currents[value.Key].priority > priority)
                return;

            _currents[value.Key] = (priority, value.Active);
        }
    }
}