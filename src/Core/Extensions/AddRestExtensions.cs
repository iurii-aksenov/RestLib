using System;
using Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static class AddRestExtensions
    {
        private const string SectionName = "Rest";

        public static IServiceCollection AddRest(this IServiceCollection services, string sectionName = SectionName)
        {
            if (services is null)
            {
                throw new ArgumentNullException($"{nameof(AddRest)}: {nameof(services)} is null.");
            }

            using var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            services.AddOptions();
            services.Configure<RestOptions>(configuration.GetSection(sectionName));
            var restSettings = configuration.GetOptions<RestOptions>(sectionName);
            services.AddSingleton(restSettings);
            services.AddHeaderPropagation(options => options.Headers.Add("Rest-TraceId"));
            return services;
        }
    }
}