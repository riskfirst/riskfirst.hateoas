using RiskFirst.Hateoas.Models;
using System;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class LinksOptions
    {
        private IDictionary<string, ILinksPolicy> PolicyMap { get; } = new Dictionary<string, ILinksPolicy>();

        public ILinksPolicy DefaultPolicy { get; set; } = new LinksPolicyBuilder<ILinkContainer>().RequireSelfLink().Build();

        public void AddPolicy<TResource>(Action<LinksPolicyBuilder<TResource>> configurePolicy) //where TResource : class
        {
            AddPolicy("Default", configurePolicy);
        }

        public void AddPolicy<TResource>(string name, Action<LinksPolicyBuilder<TResource>> configurePolicy) //where TResource : class
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Policy name cannot be null", nameof(name));
            if (configurePolicy == null)
                throw new ArgumentNullException(nameof(configurePolicy));

            var builder = new LinksPolicyBuilder<TResource>();
            configurePolicy(builder);
            var policyName = $"{name}:{typeof(TResource).FullName}";
            PolicyMap[policyName] = builder.Build();
        }
        public ILinksPolicy GetPolicy<TResource>()
        {
            return GetPolicy<TResource>("Default");
        }
        public ILinksPolicy GetPolicy<TResource>(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Policy name cannot be null", nameof(name));
            var policyName = $"{name}:{typeof(TResource).FullName}";
            return PolicyMap.ContainsKey(policyName) ? PolicyMap[policyName] as ILinksPolicy/*<TResource>*/ : null;
        }

        private ILinkTransformation hrefTransformation = new LinkTransformationBuilder().AddProtocol().AddHost().AddRoutePath().Build();
        private ILinkTransformation relTransformation = new LinkTransformationBuilder().Add(ctx => $"{ctx.LinkSpec.ControllerName}/{ctx.LinkSpec.RouteName}").Build();
        public ILinkTransformation HrefTransformation => hrefTransformation;
        public ILinkTransformation RelTransformation => relTransformation;
        public void UseHrefTransformation<T>()
            where T : ILinkTransformation, new()
        {
            UseHrefTransformation(new T());
        }

        public void UseHrefTransformation<T>(T transform)
            where T : ILinkTransformation
        {
            this.hrefTransformation = transform;
        }

        public void UseRelativeHrefs()
        {
            hrefTransformation = new LinkTransformationBuilder().AddRoutePath().Build();
        }
        public void ConfigureHrefTransformation(Action<LinkTransformationBuilder> configureTransform)
        {
            var builder = new LinkTransformationBuilder();
            configureTransform(builder);
            this.hrefTransformation = builder.Build();
        }

        public void UseRelTransformation<T>()
            where T : ILinkTransformation, new()
        {
            UseRelTransformation(new T());
        }

        public void UseRelTransformation<T>(T transform)
            where T : ILinkTransformation
        {
            this.relTransformation = transform;
        }

        public void ConfigureRelTransformation(Action<LinkTransformationBuilder> configureTransform)
        {
            var builder = new LinkTransformationBuilder();
            configureTransform(builder);
            this.relTransformation = builder.Build();
        }
    }
}
