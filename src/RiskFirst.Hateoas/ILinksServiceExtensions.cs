using RiskFirst.Hateoas.Models;
using System;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public static class ILinksServiceExtensions
    {
        public static Task AddLinksAsync<T>(this ILinksService service, T linkContainer, ILinksPolicy/*<T>*/ policy) where T : ILinkContainer
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (policy == null)
                throw new ArgumentNullException(nameof(policy));

            return service.AddLinksAsync(linkContainer, policy.Requirements);
        }

        public static Task AddLinksAsync<T>(this ILinksService service, T linkContainer, ILinksRequirement requirement) where T : ILinkContainer
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (requirement == null)
                throw new ArgumentNullException(nameof(requirement));

            return service.AddLinksAsync(linkContainer, new[] { requirement });
        }
    }
}
