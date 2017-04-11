using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace RiskFirst.Hateoas
{
    public class LinkAuthorizationContext<TResource>
    {
        public LinkAuthorizationContext(bool shouldAuthorizeRoute,
            IEnumerable<IAuthorizationRequirement> authorizationRequirements,
            IEnumerable<string> authorizationPolicyNames,
            RouteInfo routeInfo,
            RouteValueDictionary routeValues,
            TResource resource,
            ClaimsPrincipal user)
        {
            this.ShouldAuthorizeRoute = shouldAuthorizeRoute;
            this.AuthorizationRequirements = new List<IAuthorizationRequirement>(authorizationRequirements ?? Enumerable.Empty<IAuthorizationRequirement>()).AsReadOnly();
            this.AuthorizationPolicyNames = new List<string>(authorizationPolicyNames ?? Enumerable.Empty<string>()).AsReadOnly();
            this.RouteInfo = routeInfo;
            this.RouteValues = routeValues;
            this.Resource = resource;
            this.User = user;
        }
        public bool ShouldAuthorizeRoute { get; }
        public IReadOnlyList<IAuthorizationRequirement> AuthorizationRequirements { get; }
        public IReadOnlyList<string> AuthorizationPolicyNames { get; }
        public RouteInfo RouteInfo { get; }
        public RouteValueDictionary RouteValues { get; }
        public TResource Resource { get; }
        public ClaimsPrincipal User { get; }
    }
}
