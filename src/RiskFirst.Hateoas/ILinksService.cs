using RiskFirst.Hateoas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public interface ILinksService
    {
        Task AddLinksAsync<TResource>(TResource linkContainer) where TResource : ILinkContainer;
        Task AddLinksAsync<TResource>(TResource linkContainer, IEnumerable<ILinksRequirement> requirements) where TResource : ILinkContainer;
        Task AddLinksAsync<TResource>(TResource linkContainer, string policyName) where TResource : ILinkContainer;
    }
}
