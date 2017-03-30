using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Implementation
{
    public class SelfLinkRequirement<TResource> : LinksHandler<SelfLinkRequirement<TResource>, TResource>, ILinksRequirement<TResource>
        where TResource : class
    {

        public SelfLinkRequirement()
        {

        }

        public string Id { get; set; }
       
        protected override Task HandleRequirementAsync(LinksHandlerContext<TResource> context, SelfLinkRequirement<TResource> requirement)
        {
            var route = context.RouteMap.GetCurrentRoute();
            var values = context.CurrentRouteValues;

            context.Links.Add(new LinkSpec(requirement.Id, route, values));
            context.Handled(requirement);
            return Task.CompletedTask;
        }
    }
}
