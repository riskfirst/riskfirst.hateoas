using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.CustomRequirementSample.Requirement
{
    public class RootLinkRequirement<TResource> : ILinksRequirement<TResource>
    {
        public RootLinkRequirement()
        {
        }
        public string Id { get; set; } = "root";
    }
}
