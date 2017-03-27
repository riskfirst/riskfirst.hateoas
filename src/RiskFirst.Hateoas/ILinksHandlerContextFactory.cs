using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public interface ILinksHandlerContextFactory
    {
        LinksHandlerContext<TResource> CreateContext<TResource>(IEnumerable<ILinksRequirement> requirements, TResource resource);
    }
}
