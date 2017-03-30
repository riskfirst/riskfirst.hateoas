using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace RiskFirst.Hateoas
{
    public enum LinkRequirementSkipReason { Assertion, Authorization, Error, Custom }

    public class LinksHandlerContext<TResource>
    {
        private HashSet<ILinksRequirement> pendingRequirements;

        public LinksHandlerContext(
            IEnumerable<ILinksRequirement> requirements,
            IRouteMap routeMap,
            ILinkAuthorizationService authService,
            ILogger<LinksHandlerContext<TResource>> logger,
            ActionContext actionContext,
            TResource resource)
        {
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));
            if (routeMap == null)
                throw new ArgumentNullException(nameof(routeMap));
            if (authService == null)
                throw new ArgumentNullException(nameof(authService));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (actionContext == null)
                throw new ArgumentNullException(nameof(actionContext));
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            this.Requirements = requirements;
            this.RouteMap = routeMap;
            this.Authorization = authService;
            this.ActionContext = actionContext;
            this.Resource = resource;
            this.Logger = logger;
            this.pendingRequirements = new HashSet<ILinksRequirement>(requirements);
        }

        public ActionContext ActionContext { get; }

        public ILogger<LinksHandlerContext<TResource>> Logger { get; set; }

        public virtual TResource Resource { get; }

        public virtual IEnumerable<ILinksRequirement> Requirements { get; }

        public virtual HashSet<ILinksRequirement> PendingRequirements => this.pendingRequirements;

        public virtual IRouteMap RouteMap { get; }

        public ClaimsPrincipal User => ActionContext?.HttpContext?.User;

        public RouteValueDictionary CurrentRouteValues => ActionContext?.RouteData?.Values;

        public virtual IList<ILinkSpec> Links { get; } = new List<ILinkSpec>();

        public virtual ILinkAuthorizationService Authorization { get; }

        public void Handled(ILinksRequirement requirement)
        {
            pendingRequirements.Remove(requirement);
        }
        public void Skipped(ILinksRequirement requirement)
        {
            Skipped(requirement, LinkRequirementSkipReason.Custom,String.Empty);
        }
        public void Skipped(ILinksRequirement requirement, string message)
        {
            Skipped(requirement, LinkRequirementSkipReason.Custom, message);
        }
        public void Skipped(ILinksRequirement requirement, LinkRequirementSkipReason reason)
        {
            Skipped(requirement, reason, String.Empty);
        }
        public void Skipped(ILinksRequirement requirement, LinkRequirementSkipReason reason, string message)
        {
            Logger.LogInformation("Link {Requirement} skipped for user {User}. Reason: {LinkSkipReason}. {Message}.", requirement, User.Identity, reason,message);
            pendingRequirements.Remove(requirement);
        }

        public bool IsSuccess()
        {
            return pendingRequirements.Count == 0;
        }

    }

}
