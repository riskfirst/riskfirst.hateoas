using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiskFirst.Hateoas
{
    public class LinkCondition<TResource>
    {
        public static readonly LinkCondition<TResource> None = new LinkCondition<TResource>(false, Enumerable.Empty<Func<TResource, bool>>(), Enumerable.Empty<IAuthorizationRequirement>());

        public LinkCondition(bool requiresRouteAuthorization, IEnumerable<Func<TResource, bool>> assertions, IEnumerable<IAuthorizationRequirement> requirements)
        {
            this.RequiresRouteAuthorization = requiresRouteAuthorization;
            this.Assertions = new List<Func<TResource, bool>>(assertions).AsReadOnly();
            this.AuthorizationRequirements = new List<IAuthorizationRequirement>(requirements).AsReadOnly();
        }
        public IReadOnlyList<Func<TResource, bool>> Assertions { get; }
        public IReadOnlyList<IAuthorizationRequirement> AuthorizationRequirements { get; set; }
        public bool RequiresRouteAuthorization { get; set; }
        public bool RequiresAuthorization => RequiresRouteAuthorization || AuthorizationRequirements.Any();
        public bool AssertAll(TResource resource)
        {
            return !this.Assertions.Any() || this.Assertions.All(ass => ass(resource));
        }
    }
}
