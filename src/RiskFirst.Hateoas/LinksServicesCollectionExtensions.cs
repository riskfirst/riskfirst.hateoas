using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using RiskFirst.Hateoas.Polyfills;

namespace RiskFirst.Hateoas
{
    public static class LinksServicesCollectionExtensions
    {
        public static IServiceCollection AddLinks(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IAssemblyLoader, DefaultAssemblyLoader>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAdd(ServiceDescriptor.Transient<IRouteMap, DefaultRouteMap>());
            services.TryAdd(ServiceDescriptor.Transient<ILinksHandlerContextFactory, DefaultLinksHandlerContextFactory>());
            services.TryAdd(ServiceDescriptor.Transient<ILinksPolicyProvider, DefaultLinksPolicyProvider>());
            services.TryAdd(ServiceDescriptor.Transient<ILinkTransformationContextFactory, DefaultLinkTransformationContextFactory>());
            services.TryAdd(ServiceDescriptor.Transient<ILinksEvaluator, DefaultLinksEvaluator>());
            services.TryAdd(ServiceDescriptor.Transient<ILinkAuthorizationService, DefaultLinkAuthorizationService>());
            services.TryAdd(ServiceDescriptor.Transient<ILinksService, DefaultLinksService>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<ILinksHandler, Implementation.PassThroughLinksHandler>());
            return services;
        }

        public static IServiceCollection AddLinks(this IServiceCollection services, Action<LinksOptions> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.Configure(configure);
            return services.AddLinks(); ;
        }
    }
}
