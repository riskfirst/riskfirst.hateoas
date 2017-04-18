using Microsoft.AspNetCore.Routing;
using RiskFirst.Hateoas.Implementation;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RiskFirst.Hateoas
{
    public class LinksPolicyBuilder<TResource>
    {
        public LinksPolicyBuilder()
        {
        }
        private IList<ILinksRequirement> Requirements { get; } = new List<ILinksRequirement/*<TResource>*/>();

        public LinksPolicyBuilder<TResource> Combine(ILinksPolicy policy)
        {
            foreach(var requirement in policy.Requirements)
            {
                Requirements.Add(requirement);
            }
            return this;
        }
        
        public LinksPolicyBuilder<TResource> Requires<TRequirement>()
            where TRequirement : ILinksRequirement, new()
        {
            Requirements.Add(new TRequirement());
            return this;
        }
        public LinksPolicyBuilder<TResource> Requires<TRequirement>(TRequirement requirement)
            where TRequirement : ILinksRequirement
        {
            Requirements.Add(requirement);
            return this;
        }             

        public LinksPolicy Build()
        {
            return new LinksPolicy(Requirements);
        }

    }
}
