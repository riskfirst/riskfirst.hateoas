using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public enum LinkRequirementSkipReason { Assertion, Authorization, Error, Custom }

    public class LinksHandlerContext
    {
        private HashSet<ILinksRequirement> pendingRequirements;
        private ILinkAuthorizationService authService;

        public LinksHandlerContext(
            IEnumerable<ILinksRequirement> requirements,
            IRouteMap routeMap,
            ILinkAuthorizationService authService,
            ILogger<LinksHandlerContext> logger,
            ActionContext actionContext,
            object resource)
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
            this.ActionContext = actionContext;
            this.Resource = resource;
            this.Logger = logger;

            this.authService = authService;
            this.pendingRequirements = new HashSet<ILinksRequirement>(requirements);
        }

        public ActionContext ActionContext { get; }

        public ILogger<LinksHandlerContext> Logger { get; set; }

        public virtual object Resource { get; }

        public virtual IEnumerable<ILinksRequirement> Requirements { get; }

        public virtual HashSet<ILinksRequirement> PendingRequirements => this.pendingRequirements;

        public virtual IRouteMap RouteMap { get; }

        public ClaimsPrincipal User => ActionContext?.HttpContext?.User;

        public RouteInfo CurrentRoute => RouteMap.GetCurrentRoute();
        public RouteValueDictionary CurrentRouteValues => ActionContext?.RouteData?.Values;
        public IQueryCollection CurrentQueryValues => ActionContext?.HttpContext?.Request?.Query ?? new QueryCollection();

        public virtual IList<ILinkSpec> Links { get; } = new List<ILinkSpec>();
        
        public virtual bool AssertAll<TResource>(LinkCondition<TResource> condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            return !condition.Assertions.Any() || condition.Assertions.All(a => a((TResource)this.Resource));
        }

        public virtual async Task<bool> AuthorizeAsync<TResource>(RouteInfo route, RouteValueDictionary values, LinkCondition<TResource> condition)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            var authContext = new LinkAuthorizationContext<TResource>(
                    condition.RequiresRouteAuthorization,
                    condition.AuthorizationRequirements,
                    condition.AuthorizationPolicyNames,
                    route,
                    values,
                    (TResource)this.Resource,
                    this.User);

            return await authService.AuthorizeLink(authContext);
        }

        public virtual void Handled(ILinksRequirement requirement)
        {
            pendingRequirements.Remove(requirement);
        }       
        public virtual void Skipped(ILinksRequirement requirement, LinkRequirementSkipReason reason = LinkRequirementSkipReason.Custom, string message = null)
        {
            var username = User?.Identity?.Name ?? "Unknown";
            Logger.LogInformation("Link {Requirement} skipped for user {User}. Reason: {LinkSkipReason}. {Message}.", requirement, username, reason,message ?? String.Empty);
            pendingRequirements.Remove(requirement);
        }

        public virtual bool IsSuccess()
        {
            return pendingRequirements.Count == 0;
        }

    }

}
