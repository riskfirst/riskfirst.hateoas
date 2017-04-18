using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public interface ILinksHandlerContextFactory
    {
        LinksHandlerContext CreateContext(IEnumerable<ILinksRequirement> requirements, object resource);
    }
}
