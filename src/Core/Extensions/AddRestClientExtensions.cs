using System;
using Dodo.HttpClientResiliencePolicies;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static  class AddRestClientExtensions
    {
        public static IServiceCollection AddRestClient<TClient, TImplementation>(this IServiceCollection services)
            where TClient : class where TImplementation : class, TClient
        {
            if (services is null)
            {
                throw new ArgumentNullException($"{nameof(AddRestExtensions.AddRest)}: {nameof(services)} is null.");
            }

            services.AddHttpClient<TClient, TImplementation>().AddResiliencePolicies();
            return services;
        }
    }
}