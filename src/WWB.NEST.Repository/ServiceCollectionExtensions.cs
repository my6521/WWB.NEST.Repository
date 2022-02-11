using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace  WWB.NEST.Repository
{
    public static class ServiceCollectionExtensions
    {
     
        public static IServiceCollection AddNESTRepository(this IServiceCollection services,Action<ElasticOptions>  setup)
        {
            var options = new ElasticOptions();
            setup?.Invoke(options);

            services.AddSingleton(options);
            
            services.AddSingleton<IElasticClientProvider, ElasticClientProvider>();
            
            return services;
        }
    }
}