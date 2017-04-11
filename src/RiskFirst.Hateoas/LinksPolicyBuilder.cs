using Microsoft.AspNetCore.Routing;
using RiskFirst.Hateoas.Implementation;
using System;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class LinksPolicyBuilder<TResource>
    {
        public LinksPolicyBuilder()
        {
        }
        private IList<ILinksRequirement<TResource>> Requirements { get; } = new List<ILinksRequirement<TResource>>();

        
        public LinksPolicyBuilder<TResource> Requires<TRequirement>()
            where TRequirement : ILinksRequirement<TResource>, new()
        {
            Requirements.Add(new TRequirement());
            return this;
        }
        public LinksPolicyBuilder<TResource> Requires<TRequirement>(TRequirement requirement)
            where TRequirement : ILinksRequirement<TResource>
        {
            Requirements.Add(requirement);
            return this;
        }             

        public LinksPolicy<TResource> Build()
        {
            return new LinksPolicy<TResource>(Requirements);
        }

    }
}
