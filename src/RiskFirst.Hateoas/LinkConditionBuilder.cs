using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiskFirst.Hateoas
{
    public class LinkConditionBuilder<TResource>
    {

        private bool requiresRouteAuthorization = false;
        private List<IAuthorizationRequirement> authRequirements = new List<IAuthorizationRequirement>();
        private List<string> authPolicyNames = new List<string>();
        private IList<Func<TResource, bool>> assertions = new List<Func<TResource, bool>>();
        public LinkConditionBuilder()
        {
        }

        public LinkConditionBuilder<TResource> AuthorizeRoute()
        {
            this.requiresRouteAuthorization = true;
            return this;
        }
        public LinkConditionBuilder<TResource> Authorize(params IAuthorizationRequirement [] requirements)
        {
            authRequirements.AddRange(requirements);
            return this;
        }
        public LinkConditionBuilder<TResource> Authorize(params AuthorizationPolicy[] policies)
        {
            authRequirements.AddRange(policies.SelectMany(p => p.Requirements));
            return this;
        }
        public LinkConditionBuilder<TResource> Authorize(params string[] policyNames)
        {
            authPolicyNames.AddRange(policyNames);
            return this;
        }

        public LinkConditionBuilder<TResource> Assert(Func<TResource, bool> condition)
        {
            this.assertions.Add(condition);
            return this;
        }

        public LinkCondition<TResource> Build()
        {
            return new LinkCondition<TResource>(this.requiresRouteAuthorization, this.assertions, this.authRequirements, this.authPolicyNames);
        }
    }
}
