using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Implementation
{
    public class RouteLinkRequirement<TResource> : LinksHandler<RouteLinkRequirement<TResource>, TResource>, ILinksRequirement<TResource>
       where TResource : class
    {
        public RouteLinkRequirement()
        {

        }
        public string Id { get; set; }
        public string RouteName { get; set; }
        public Func<TResource, RouteValueDictionary> GetRouteValues { get; set; }
        public LinkCondition<TResource> Condition { get; set; } = LinkCondition<TResource>.None;
        public ILinksRequirement<T> Convert<T>() where T : class
        {
            return new RouteLinkRequirement<T>()
            {
                Id = this.Id,
                GetRouteValues = this.GetRouteValues == null ? (Func<T, RouteValueDictionary>)null : r => this.GetRouteValues(r as TResource),
                RouteName = this.RouteName,
                Condition = new LinkCondition<T>(this.Condition.RequiresAuthorization,
                                                this.Condition.Assertions.Select(a => new Func<T, bool>(x => a(x as TResource))),
                                                this.Condition.Policies)
            };
        }

        protected override async Task HandleRequirementAsync(LinksHandlerContext<TResource> context, RouteLinkRequirement<TResource> requirement)
        {
            var condition = requirement.Condition;
            if (condition != null && !condition.AssertAll(context.Resource))
            {
                context.Skipped(requirement, LinkRequirementSkipReason.Assertion);
                return;
            }

            var route = context.RouteMap.GetRoute(requirement.RouteName);
            if(route == null)
            {
                context.Skipped(requirement, LinkRequirementSkipReason.Error);
                return;
            }
            var values = new RouteValueDictionary();
            if (requirement.GetRouteValues != null)
            {
                values = requirement.GetRouteValues(context.Resource);
            }
            if (condition != null && condition.RequiresAuthorization)
            {
                var authContext = new LinkAuthorizationContext<TResource>(
                    condition.RequiresRouteAuthorization,
                    condition.Policies,
                    route,
                    values,
                    context.Resource,
                    context.User);

                if (!await context.Authorization.AuthorizeLink(authContext))
                {
                    context.Skipped(requirement, LinkRequirementSkipReason.Authorization);
                    return;
                }
            }

            context.Links.Add(new LinkSpec()
            {
                Id = requirement.Id,
                Values = values,
                RouteName = route.RouteName,
                ControllerName = route.ControllerName,
                Method = route.HttpMethod
            });
            context.Handled(requirement);
        }
    }
}
