using RiskFirst.Hateoas.CustomRequirementSample.Requirement;

namespace RiskFirst.Hateoas.CustomRequirementSample
{
    public static class LinksPolicyBuiderExtensions
    {
        public static LinksPolicyBuilder<TResource> RequiresApiRootLink<TResource>(this LinksPolicyBuilder<TResource> builder)
            where TResource : class
        {
            return builder.Requires<ApiRootLinkRequirement>();
        }
    }
}
