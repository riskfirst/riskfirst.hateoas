using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class LinkConditionBuilder<TResource> where TResource : class
    {

        private bool requiresRouteAuthorization = false;
        private List<IAuthorizationRequirement> authRequirements = new List<IAuthorizationRequirement>();
        private IList<Func<TResource, bool>> assertions = new List<Func<TResource, bool>>();
        public LinkConditionBuilder()
        {
        }

        public LinkConditionBuilder<TResource> AuthorizeRoute()
        {
            this.requiresRouteAuthorization = true;
            return this;
        }
        public LinkConditionBuilder<TResource> AuthorizeRequirements(params IAuthorizationRequirement [] requirements)
        {
            authRequirements.AddRange(requirements);
            return this;
        }
        public LinkConditionBuilder<TResource> AuthorizePolicy(AuthorizationPolicy policy)
        {
            authRequirements.AddRange(policy.Requirements);
            return this;
        }


        public LinkConditionBuilder<TResource> Assert(Func<TResource, bool> condition)
        {
            this.assertions.Add(condition);
            return this;
        }

        public LinkCondition<TResource> Build()
        {
            return new LinkCondition<TResource>(this.requiresRouteAuthorization, this.assertions, this.authRequirements);
        }
    }
}
