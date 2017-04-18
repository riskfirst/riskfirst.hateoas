using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Implementation
{
    public class RouteLinkRequirement<TResource> : LinksHandler<RouteLinkRequirement<TResource>>, ILinksRequirement
    {
        public RouteLinkRequirement()
        {

        }
        public string Id { get; set; }
        public string RouteName { get; set; }
        public Func<TResource, RouteValueDictionary> GetRouteValues { get; set; }
        public LinkCondition<TResource> Condition { get; set; } = LinkCondition<TResource>.None;
        
        protected override async Task HandleRequirementAsync(LinksHandlerContext context, RouteLinkRequirement<TResource> requirement)
        {
            var condition = requirement.Condition;
            if (!context.AssertAll(condition))
            {
                context.Skipped(requirement, LinkRequirementSkipReason.Assertion);
                return;
            }
            if(String.IsNullOrEmpty(requirement.RouteName))
            {
                context.Skipped(requirement, LinkRequirementSkipReason.Error, $"Requirement did not have a RouteName specified for link: {requirement.Id}");
                return;
            }

            var route = context.RouteMap.GetRoute(requirement.RouteName);
            if(route == null)
            {
                context.Skipped(requirement, LinkRequirementSkipReason.Error,$"No route was found for route name: {requirement.RouteName}");
                return;
            }
            var values = new RouteValueDictionary();
            if (requirement.GetRouteValues != null)
            {
                values = requirement.GetRouteValues((TResource)context.Resource);
            }
            if (condition != null && condition.RequiresAuthorization)
            {    
                if (!await context.AuthorizeAsync(route,values,condition))
                {
                    context.Skipped(requirement, LinkRequirementSkipReason.Authorization);
                    return;
                }
            }

            context.Links.Add(new LinkSpec(requirement.Id, route, values));
            context.Handled(requirement);
        }
    }
}
