using System;
using System.Threading.Tasks;

namespace Toggles
{
    public static class ToggleExtensions
    {
        public static ToggleExecutor If(this IToggle toggle, string key)
        {
            return new ToggleExecutor(toggle.IsOn(key));
        }
    }

    public class ToggleExecutor
    {
        private readonly bool _isOn;

        internal ToggleExecutor(bool isOn)
        {
            _isOn = isOn;
        }

        public ToggleExecutor Then(Action action)
        {
            if (_isOn)
                action?.Invoke();

            return this;
        }

        public ToggleExecutor Else(Action action)
        {
            if(!_isOn)
                action?.Invoke();

            return this;
        }
    }
}
