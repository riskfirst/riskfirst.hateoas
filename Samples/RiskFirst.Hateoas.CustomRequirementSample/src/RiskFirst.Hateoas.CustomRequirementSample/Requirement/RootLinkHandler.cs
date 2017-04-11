using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.CustomRequirementSample.Requirement
{
    public class RootLinkHandler : ILinksHandler
    {
        public Task HandleAsync<T>(LinksHandlerContext<T> context)
        { 
            var route = context.RouteMap.GetRoute("ApiRoot"); // Asumes your controller has a named route "ApiRoot".
            foreach(var requirement in context.Requirements.OfType<RootLinkRequirement<T>>())
            {
                context.Links.Add(new LinkSpec(requirement.Id, route, null));
                context.Handled(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
