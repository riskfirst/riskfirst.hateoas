using RiskFirst.Hateoas.Models;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public interface ILinksPolicyProvider
    {
        Task<ILinksPolicy> GetDefaultPolicyAsync();
        Task<ILinksPolicy> GetPolicyAsync<TResource>();
        Task<ILinksPolicy> GetPolicyAsync<TResource>(string policyName);
    }
}
