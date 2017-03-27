using System;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class LinksPolicy<TResource> : ILinksPolicy<TResource> where TResource : class
    {
        public LinksPolicy(IEnumerable<ILinksRequirement<TResource>> requirements)
        {
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));

            this.Requirements = new List<ILinksRequirement<TResource>>(requirements).AsReadOnly();
        }
        public IReadOnlyList<ILinksRequirement<TResource>> Requirements { get; }

    }
}
