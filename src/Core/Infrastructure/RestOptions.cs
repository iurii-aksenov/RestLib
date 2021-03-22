using System;
using System.Collections.Generic;

namespace Core.Infrastructure
{
    public class RestOptions
    {
        public RestToken RestToken { get; set; } = null!;
        public List<ServiceRoute> Services { get; set; } = new();

        public Uri GetUri(string serviceName)
        {
            var service = Services.Find(x => x.Name.StartsWith(serviceName, StringComparison.OrdinalIgnoreCase));
            if (service is null)
            {
                throw new ArgumentNullException($"There is no service with name {serviceName}");
            }

            return new UriBuilder
            {
                Host = service.Host,
                Scheme = service.Scheme,
                Port = service.Port == 0 ? -1 : service.Port
            }.Uri;
        }
    }

    public class RestToken
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string IssuerSigningKey { get; set; } = null!;
    }

    public class ServiceRoute
    {
        public string Name { get; set; } = null!;
        public string Scheme { get; set; } = null!;
        public string Host { get; set; } = null!;
        public int Port { get; set; }
    }
}