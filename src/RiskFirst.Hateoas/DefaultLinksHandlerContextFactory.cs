using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class DefaultLinksHandlerContextFactory : ILinksHandlerContextFactory
    {
        private readonly IRouteMap routeMap;
        private readonly ILinkAuthorizationService authService;
        private readonly IHttpContextAccessor httpAccessor;

        public DefaultLinksHandlerContextFactory(IRouteMap routeMap, ILinkAuthorizationService authService, IHttpContextAccessor httpAccessor, IActionContextAccessor actionAccessor)
        {
            this.routeMap = routeMap;
            this.authService = authService;
            this.httpAccessor = httpAccessor;
        }
        public LinksHandlerContext<TResource> CreateContext<TResource>(IEnumerable<ILinksRequirement> requirements, TResource resource)
        {
            return new LinksHandlerContext<TResource>(requirements, routeMap, authService, httpAccessor.HttpContext.User, resource);
        }
    }
}
