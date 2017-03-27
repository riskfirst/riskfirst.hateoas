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

        public ILinksRequirement<T> Convert<T>() where T : class
        {
            return new SelfLinkRequirement<T>()
            {
                Id = this.Id
            };
        }
        protected override Task HandleRequirementAsync(LinksHandlerContext<TResource> context, SelfLinkRequirement<TResource> requirement)
        {
            var route = context.RouteMap.GetCurrentRoute();
            var values = context.RouteMap.GetCurrentRouteValues();

            context.Links.Add(new LinkSpec()
            {
                Id = requirement.Id,
                RouteName = route.RouteName,
                ControllerName = route.ControllerName,
                Values = values,
                Method = route.HttpMethod
            });
            context.Handled(requirement);
            return Task.CompletedTask;
        }
    }
}
