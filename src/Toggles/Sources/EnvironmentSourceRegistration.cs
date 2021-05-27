using System;

namespace Toggles.Sources
{
    public static class EnvironmentSourceRegistration
    {
        public static SourcesRegister AddEnvironmentSource(this SourcesRegister register, int priority, EnvironmentVariableTarget target, string prefix = null)
        {
            register.AddSource<EnvironmentSource>(priority, provider => new EnvironmentSource(target, prefix));
            return register;
        }
    }
}