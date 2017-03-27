using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Security.Claims;

namespace RiskFirst.Hateoas
{
    public class LinkAuthorizationContext<TResource>
    {
        public LinkAuthorizationContext(bool shouldAuthorizeRoute,
            IEnumerable<string> authorizePolicies,
            RouteInfo routeInfo,
            RouteValueDictionary routeValues,
            TResource resource,
            ClaimsPrincipal user)
        {
            this.ShouldAuthorizeRoute = shouldAuthorizeRoute;
            this.AuthorizePolicies = new List<string>(authorizePolicies).AsReadOnly();
            this.RouteInfo = routeInfo;
            this.RouteValues = routeValues;
            this.User = user;
        }
        public bool ShouldAuthorizeRoute { get; }
        public IReadOnlyList<string> AuthorizePolicies { get; }
        public RouteInfo RouteInfo { get; }
        public RouteValueDictionary RouteValues { get; }
        public TResource Resource { get; }
        public ClaimsPrincipal User { get; }
    }
}
