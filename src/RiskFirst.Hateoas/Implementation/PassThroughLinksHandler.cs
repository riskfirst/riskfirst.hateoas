using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Implementation
{
    public class PassThroughLinksHandler : ILinksHandler
    {
        public async Task HandleAsync<T>(LinksHandlerContext<T> context)
        {
            foreach (var handler in context.Requirements.OfType<ILinksHandler<T>>())
            {
                if (context.PendingRequirements.Contains((ILinksRequirement)handler))
                    await handler.HandleAsync(context);
            }
        }
    }
}
