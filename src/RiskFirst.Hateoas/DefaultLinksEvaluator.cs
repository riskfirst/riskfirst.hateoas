using RiskFirst.Hateoas.Models;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class DefaultLinksEvaluator : ILinksEvaluator
    {
        private readonly ILinkHelper linkHelper;
        public DefaultLinksEvaluator(ILinkHelper linkHelper)
        {
            this.linkHelper = linkHelper;
        }
        public void BuildLinks(IEnumerable<LinkSpec> links, ILinkContainer container)
        {
            foreach (var link in links)
            {
                container.AddLink(link.Id, new Link()
                {
                    Href = linkHelper.GetHref(link),
                    Rel = linkHelper.GetRel(link),
                    Method = link.Method.ToString()
                });
            }
        }

    }
}
