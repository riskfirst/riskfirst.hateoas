using Microsoft.Extensions.Options;
using RiskFirst.Hateoas.Models;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class DefaultLinksEvaluator : ILinksEvaluator
    {
        private readonly LinksOptions options;
        private readonly ILinkTransformationContextFactory contextFactory;
        public DefaultLinksEvaluator(IOptions<LinksOptions> options, ILinkTransformationContextFactory contextFactory)
        {
            this.options = options.Value;
            this.contextFactory = contextFactory;
        }
        public void BuildLinks(IEnumerable<LinkSpec> links, ILinkContainer container)
        {
            foreach (var link in links)
            {
                var context = contextFactory.CreateContext(link);
                container.AddLink(link.Id, new Link()
                {
                    Href = options.HrefTransformation?.Transform(context),
                    Rel = options.RelTransformation?.Transform(context),
                    Method = link.HttpMethod.ToString()
                });
            }
        }

    }
}
