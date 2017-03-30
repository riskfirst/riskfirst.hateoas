using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class DefaultLinksHandlerContextFactory : ILinksHandlerContextFactory
    {
        private readonly IRouteMap routeMap;
        private readonly ILinkAuthorizationService authService;
        private readonly ActionContext actionContext;
        private readonly ILoggerFactory loggerFactory;

        public DefaultLinksHandlerContextFactory(IRouteMap routeMap, ILinkAuthorizationService authService, IActionContextAccessor actionAccessor, ILoggerFactory loggerFactory)
        {
            this.routeMap = routeMap;
            this.authService = authService;
            this.actionContext = actionAccessor.ActionContext;
            this.loggerFactory = loggerFactory;
        }
        public LinksHandlerContext<TResource> CreateContext<TResource>(IEnumerable<ILinksRequirement> requirements, TResource resource)
        {
            var logger = loggerFactory.CreateLogger<LinksHandlerContext<TResource>>();
            return new LinksHandlerContext<TResource>(requirements, routeMap, authService,logger, actionContext, resource);
        }
    }
}
