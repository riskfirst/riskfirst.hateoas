using System;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class LinkConditionBuilder<TResource> where TResource : class
    {

        private bool requiresRouteAuthorization = false;
        private IList<string> policies = new List<string>();
        private IList<Func<TResource, bool>> assertions = new List<Func<TResource, bool>>();
        public LinkConditionBuilder()
        {
        }

        public LinkConditionBuilder<TResource> AuthorizeRoute()
        {
            this.requiresRouteAuthorization = true;
            return this;
        }

        public LinkConditionBuilder<TResource> AuthorizePolicy(string policy)
        {
            policies.Add(policy);
            return this;
        }


        public LinkConditionBuilder<TResource> Assert(Func<TResource, bool> condition)
        {
            this.assertions.Add(condition);
            return this;
        }

        public LinkCondition<TResource> Build()
        {
            return new LinkCondition<TResource>(this.requiresRouteAuthorization, this.assertions, this.policies);
        }
    }
}
