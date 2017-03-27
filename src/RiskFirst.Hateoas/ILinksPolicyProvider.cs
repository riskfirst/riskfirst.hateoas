using RiskFirst.Hateoas.Models;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public interface ILinksPolicyProvider
    {
        Task<ILinksPolicy<ILinkContainer>> GetDefaultPolicyAsync();
        Task<ILinksPolicy<TResource>> GetPolicyAsync<TResource>();
        Task<ILinksPolicy<TResource>> GetPolicyAsync<TResource>(string policyName);
    }
}
