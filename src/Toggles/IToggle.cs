using System;
using System.Collections.Generic;

namespace Toggles
{
    public interface IToggle
    {
        bool IsOn(string key);
        IEnumerable<(int priority, Type sourceType, IToggleSource sourceInstance)> Sources { get; }
        IDictionary<string, bool> CurrentStates { get; }
    }
}
