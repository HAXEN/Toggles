using System;

namespace Toggles.Consul
{
    public class ConsulSettings
    {
        public string Host { get; set; } = Environment.GetEnvironmentVariable("CONSUL_HTTP_HOST") ?? "http://localhost";
        public string Port { get; set; } = Environment.GetEnvironmentVariable("CONSUL_HTTP_PORT") ?? "8500";
        public string FeaturesKey { get; set; }
        public string Token { get; set; } = Environment.GetEnvironmentVariable("CONSUL_HTTP_TOKEN");
        public bool DisableServerCertificateValidation { get; set; }

        public ConsulSettings()
        {
            if (bool.TryParse(Environment.GetEnvironmentVariable("CONSUL_HTTP_UNSIGNED_CERTIFICATE"), out var unsignedCert))
                DisableServerCertificateValidation = unsignedCert;
        }
    }
}