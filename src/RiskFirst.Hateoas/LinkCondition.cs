using System;
using System.Collections.Generic;
using System.Linq;

namespace RiskFirst.Hateoas
{
    public class LinkCondition<TResource>
    {
        public static readonly LinkCondition<TResource> None = new LinkCondition<TResource>(false, Enumerable.Empty<Func<TResource, bool>>(), Enumerable.Empty<string>());

        public LinkCondition(bool requiresRouteAuthorization, IEnumerable<Func<TResource, bool>> assertions, IEnumerable<string> policies)
        {
            this.RequiresRouteAuthorization = requiresRouteAuthorization;
            this.Assertions = new List<Func<TResource, bool>>(assertions).AsReadOnly();
            this.Policies = new List<string>(policies).AsReadOnly();
        }
        public IReadOnlyList<Func<TResource, bool>> Assertions { get; }
        public IReadOnlyList<string> Policies { get; set; }
        public bool RequiresRouteAuthorization { get; set; }
        public bool RequiresAuthorization => RequiresRouteAuthorization || Policies.Any();
        public bool AssertAll(TResource resource)
        {
            return !this.Assertions.Any() || this.Assertions.All(ass => ass(resource));
        }
    }
}
