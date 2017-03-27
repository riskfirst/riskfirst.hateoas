using RiskFirst.Hateoas.Models;
using System;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class LinksOptions
    {
        private IDictionary<string, ILinksPolicy> PolicyMap { get; } = new Dictionary<string, ILinksPolicy>();

        public ILinksPolicy<ILinkContainer> DefaultPolicy { get; set; } = new LinksPolicyBuilder<ILinkContainer>().RequireSelfLink().Build();

        public void AddPolicy<TResource>(Action<LinksPolicyBuilder<TResource>> configurePolicy) where TResource : class
        {
            AddPolicy(typeof(TResource).FullName, configurePolicy);
        }

        public void AddPolicy<TResource>(string name, Action<LinksPolicyBuilder<TResource>> configurePolicy) where TResource : class
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Policy name cannot be null", nameof(name));
            if (configurePolicy == null)
                throw new ArgumentNullException(nameof(configurePolicy));

            var builder = new LinksPolicyBuilder<TResource>();
            configurePolicy(builder);
            PolicyMap[name] = builder.Build();
        }
        public ILinksPolicy<TResource> GetPolicy<TResource>()
        {
            return GetPolicy<TResource>(typeof(TResource).FullName);
        }
        public ILinksPolicy<TResource> GetPolicy<TResource>(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Policy name cannot be null", nameof(name));
            return PolicyMap.ContainsKey(name) ? PolicyMap[name] as ILinksPolicy<TResource> : null;
        }
    }
}
