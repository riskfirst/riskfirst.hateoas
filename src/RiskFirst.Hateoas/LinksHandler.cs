using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public abstract class LinksHandler<TRequirement, TResource> : ILinksHandler<TResource>
        where TRequirement : ILinksRequirement
        where TResource : class
    {

        public async Task HandleAsync(LinksHandlerContext<TResource> context)
        {
            foreach (var req in context.Requirements.OfType<TRequirement>())
            {
                await HandleRequirementAsync(context, req);
            }
        }

        public async Task HandleAsync<T>(LinksHandlerContext<T> context)
        {
            if (!typeof(T).IsAssignableFrom(typeof(TResource)))
                throw new InvalidOperationException($"Invalid resource type in LinksHandler. Expected: {typeof(TResource).FullName} Actual: {typeof(T).FullName}");

            await HandleAsync(context as LinksHandlerContext<TResource>);
        }

        protected abstract Task HandleRequirementAsync(LinksHandlerContext<TResource> context, TRequirement requirement);
    }
}
