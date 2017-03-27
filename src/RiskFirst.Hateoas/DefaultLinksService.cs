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
        private readonly ILinksHandlerContextFactory contextFactory;
        private readonly ILinksPolicyProvider policyProvider;
        private readonly IList<ILinksHandler> handlers;
        private readonly IRouteMap routeMap;
        private readonly ILinksEvaluator evaluator;

        public DefaultLinksService(IOptions<LinksOptions> options, ILinksHandlerContextFactory contextFactory,
            ILinksPolicyProvider policyProvider, IEnumerable<ILinksHandler> handlers, IRouteMap routeMap, ILinksEvaluator evaluator)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
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
            if (currentRoute.LinksAttributes.Any())
            {
                foreach (var policyName in currentRoute.LinksAttributes.Select(a => a.Policy))
                {
                    var controllerPolicy = await policyProvider.GetPolicyAsync<TResource>(policyName);
                    if (controllerPolicy == null)
                        throw new InvalidOperationException($"Controller-registered Policy not defined: {policyName} Route: {currentRoute.RouteName}");
                    await this.AddLinksAsync(linkContainer, controllerPolicy);
                }
                return;
            }

            // look for policies defined on the type
            var typeAttributes = typeof(TResource).GetTypeInfo().GetCustomAttributes<LinksAttribute>(true);
            if (typeAttributes.Any())
            {
                foreach (var attr in typeAttributes)
                {
                    var typePolicy = await policyProvider.GetPolicyAsync<TResource>(attr.Policy);
                    if (typePolicy == null)
                        throw new InvalidOperationException($"Type-registered policy not defined: {attr.Policy} Type: {typeof(TResource).FullName}");
                    await this.AddLinksAsync(linkContainer, typePolicy);
                }
                return;
            }

            // look for a policy registered against the type
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

            var ctx = contextFactory.CreateContext(requirements, linkContainer);
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(ctx);
            }

            evaluator.BuildLinks(ctx.Links, linkContainer);
        }


    }
}
