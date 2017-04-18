using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Implementation
{
    public class SelfLinkRequirement<TResource> : LinksHandler<SelfLinkRequirement<TResource>>, ILinksRequirement
    {

        public SelfLinkRequirement()
        {

        }

        public string Id { get; set; }
       
        protected override Task HandleRequirementAsync(LinksHandlerContext context, SelfLinkRequirement<TResource> requirement)
        {
            var route = context.CurrentRoute;
            var values = context.CurrentRouteValues;

            context.Links.Add(new LinkSpec(requirement.Id, route, values));
            context.Handled(requirement);
            return Task.CompletedTask;
        }
    }
}
