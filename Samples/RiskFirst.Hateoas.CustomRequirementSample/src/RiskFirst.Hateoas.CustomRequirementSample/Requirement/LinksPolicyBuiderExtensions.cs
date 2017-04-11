using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.CustomRequirementSample
{
    public static class LinksPolicyBuiderExtensions
    {
        public static LinksPolicyBuilder<TResource> RequiresApiRootLink<TResource>(this LinksPolicyBuilder<TResource> builder)
            where TResource : class
        {
            return builder.Requires<Requirement.RootLinkRequirement<TResource>>();
        }
    }
}
