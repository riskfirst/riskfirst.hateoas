using Microsoft.Extensions.Options;
using RiskFirst.Hateoas.Models;
using System;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public class DefaultLinksPolicyProvider : ILinksPolicyProvider
    {
        private readonly LinksOptions options;

        public DefaultLinksPolicyProvider(IOptions<LinksOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.options = options.Value;
        }
        public Task<ILinksPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(options.DefaultPolicy);
        }

        public Task<ILinksPolicy> GetPolicyAsync<TResource>()
        {
            return Task.FromResult(options.GetPolicy<TResource>());
        }

        public Task<ILinksPolicy> GetPolicyAsync<TResource>(string policyName)
        {
            return Task.FromResult(options.GetPolicy<TResource>(policyName));
        }
    }
}
