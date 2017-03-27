using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace RiskFirst.Hateoas
{
    public enum LinkRequirementSkipReason { Assertion, Authorization }

    public class LinksHandlerContext<TResource>
    {
        private HashSet<ILinksRequirement> pendingRequirements;

        public LinksHandlerContext(
            IEnumerable<ILinksRequirement> requirements,
            IRouteMap routeMap,
            ILinkAuthorizationService authService,
            ClaimsPrincipal user,
            TResource resource)
        {
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));
            if (routeMap == null)
                throw new ArgumentNullException(nameof(routeMap));
            if (authService == null)
                throw new ArgumentNullException(nameof(authService));
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            this.Requirements = requirements;
            this.RouteMap = routeMap;
            this.Authorization = authService;
            this.User = user;
            this.Resource = resource;
            this.pendingRequirements = new HashSet<ILinksRequirement>(requirements);
        }

        public virtual TResource Resource { get; }

        public virtual IEnumerable<ILinksRequirement> Requirements { get; }

        public virtual HashSet<ILinksRequirement> PendingRequirements => this.pendingRequirements;

        public virtual IRouteMap RouteMap { get; }

        public virtual ClaimsPrincipal User { get; }

        public virtual IList<LinkSpec> Links { get; } = new List<LinkSpec>();

        public virtual ILinkAuthorizationService Authorization { get; }

        public void Handled(ILinksRequirement requirement)
        {
            pendingRequirements.Remove(requirement);
        }

        public void Skipped(ILinksRequirement requirement, LinkRequirementSkipReason reason)
        {
            pendingRequirements.Remove(requirement);
        }

    }

}
