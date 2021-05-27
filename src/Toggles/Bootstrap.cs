using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Toggles.Impl;

namespace Toggles
{
    public static class Bootstrap
    {
        private static SourcesRegister _register;

        public static IServiceCollection AddToggles(this IServiceCollection services, Action<SourcesRegister> sources = null)
        {
            _register = new SourcesRegister();
            sources?.Invoke(_register);

            foreach (var source in _register.Sources)
            {
                if(source.factory != null)
                    continue;

                services.AddSingleton(source.source);
            }
            services.AddSingleton<IToggle>(provider =>
            {
                var sources = _register.Sources.Select(x =>
                {
                    IToggleSource instance = null;
                    
                    if (x.factory != null)
                        instance = x.factory.Invoke(provider);

                    instance ??= provider.GetRequiredService(x.source) as IToggleSource;

                    return (x.priority, instance);
                }).ToArray();

                return new ToggleManager(sources);
            });

            return services;
        }
    }
}
