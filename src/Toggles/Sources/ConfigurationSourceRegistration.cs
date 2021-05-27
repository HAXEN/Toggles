using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Toggles.Sources
{
    public static class ConfigurationSourceRegistration
    {
        public static SourcesRegister AddConfigurationSource(this SourcesRegister register, int priority, string featuresSection)
        {
            register.AddSource<ConfigurationSource>(priority, provider => new ConfigurationSource(provider.GetRequiredService<IConfiguration>(), featuresSection));
            return register;
        }
    }
}
