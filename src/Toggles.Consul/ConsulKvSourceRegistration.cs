using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Toggles.Consul
{
    public static class ConsulKvSourceRegistration
    {
        public static SourcesRegister AddConsulSource(this SourcesRegister register, int priority, Action<ConsulSettings> settings)
        {
            var consulSettings = new ConsulSettings();
            settings?.Invoke(consulSettings);

            register.AddSource<ConsulKvSource>(priority, provider => new ConsulKvSource(new ConsulPoller(consulSettings, provider.GetService<ILogger<ConsulPoller>>())));

            return register;
        }
    }
}