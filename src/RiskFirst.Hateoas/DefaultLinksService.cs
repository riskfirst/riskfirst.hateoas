using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RiskFirst.Hateoas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public class DefaultLinksService : ILinksService
    {
        private readonly LinksOptions options;
        private readonly ILogger<DefaultLinksService> logger;
        private readonly ILinksHandlerContextFactory contextFactory;
        private readonly ILinksPolicyProvider policyProvider;
        private readonly IList<ILinksHandler> handlers;
        private readonly IRouteMap routeMap;
        private readonly ILinksEvaluator evaluator;

        public DefaultLinksService(
            IOptions<LinksOptions> options, 
            ILogger<DefaultLinksService> logger, 
            ILinksHandlerContextFactory contextFactory,
            ILinksPolicyProvider policyProvider, 
            IEnumerable<ILinksHandler> handlers, 
            IRouteMap routeMap, 
            ILinksEvaluator evaluator)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));
            if (policyProvider == null)
                throw new ArgumentNullException(nameof(policyProvider));
            if (handlers == null)
                throw new ArgumentNullException(nameof(handlers));
            if (routeMap == null)
                throw new ArgumentNullException(nameof(routeMap));
            if (evaluator == null)
                throw new ArgumentNullException(nameof(evaluator));

            this.options = options.Value;
            this.logger = logger;
            this.contextFactory = contextFactory;
            this.policyProvider = policyProvider;
            this.handlers = handlers.ToArray();
            this.routeMap = routeMap;
            this.evaluator = evaluator;
        }
        public async Task AddLinksAsync<TResource>(TResource linkContainer) where TResource : ILinkContainer
        {
            // look for policies defined on the controller, this is used to override policies defined on the type or registered
            var currentRoute = routeMap.GetCurrentRoute();
            if (currentRoute.LinksAttributes != null && currentRoute.LinksAttributes.Any())
            {
                foreach (var policyName in currentRoute.LinksAttributes.Select(a => a.Policy))
                {
                    var controllerPolicy = string.IsNullOrEmpty(policyName)
                            ? await policyProvider.GetPolicyAsync<TResource>()
                            : await policyProvider.GetPolicyAsync<TResource>(policyName);
                    if (controllerPolicy == null)
                        throw new InvalidOperationException($"Controller-registered Policy not defined: {policyName ?? "<default>"} Route: {currentRoute.RouteName}");
                    await this.AddLinksAsync(linkContainer, controllerPolicy);
                }
                return;
            }

            // look for policies defined on the type
            var typeAttributes = typeof(TResource).GetTypeInfo().GetCustomAttributes<LinksAttribute>(true);
            if (typeAttributes.Any())
            {
                foreach (var policyName in typeAttributes.Select(a => a.Policy))
                {
                    var typePolicy = string.IsNullOrEmpty(policyName)
                            ? await policyProvider.GetPolicyAsync<TResource>()
                            : await policyProvider.GetPolicyAsync<TResource>(policyName);
                    if (typePolicy == null)
                        throw new InvalidOperationException($"Type-registered policy not defined: {policyName ?? "<default>"} Type: {typeof(TResource).FullName}");
                    await this.AddLinksAsync(linkContainer, typePolicy);
                }
                return;
            }

            // look for a policy registered againt the type
            var defaultTypePolicy = await policyProvider.GetPolicyAsync<TResource>();
            if (defaultTypePolicy != null)
            {
                await this.AddLinksAsync(linkContainer, defaultTypePolicy);
                return;
            }

            // fallback to the default policy
            var policy = await policyProvider.GetDefaultPolicyAsync();
            await this.AddLinksAsync(linkContainer, policy);

        }
        public async Task AddLinksAsync<TResource>(TResource linkContainer, string policyName) where TResource : ILinkContainer
        {
            if (policyName == null)
                throw new ArgumentNullException(nameof(policyName));


            var policy = await policyProvider.GetPolicyAsync<TResource>(policyName);
            if (policy == null)
                throw new InvalidOperationException($"Policy not defined: {policyName}");

            await this.AddLinksAsync(linkContainer, policy);
        }

        public async Task AddLinksAsync<TResource>(TResource linkContainer, IEnumerable<ILinksRequirement> requirements) where TResource : ILinkContainer
        {
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));

            logger.LogInformation("Applying links to {ResourceType} using requirements {Requirements}", typeof(TResource).FullName, requirements);

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var ctx = contextFactory.CreateContext(requirements, linkContainer);
            foreach (var handler in handlers)
            {
                try
                {
                    await handler.HandleAsync(ctx);
                }
                catch(InvalidCastException)
                {
                    throw;
                }
                catch(Exception ex)
                {
                    logger.LogWarning("Unhandled exception in {Handler}. Exception: {Exception}. Context: {Context}", handler, ex, ctx);
                }
            }
            if(!ctx.IsSuccess())
            {
                logger.LogWarning("Not all links were handled correctly for resource {ResourceType}. Unhandled requirements: {PendingRequirements}", typeof(TResource).FullName, ctx.PendingRequirements.ToArray());
            }
            evaluator.BuildLinks(ctx.Links, linkContainer);
            sw.Stop();

            logger.LogInformation("Applied links to {ResourceType} in {ElapsedMilliseconds}ms", typeof(TResource).FullName, sw.ElapsedMilliseconds);
        }


    }
}
