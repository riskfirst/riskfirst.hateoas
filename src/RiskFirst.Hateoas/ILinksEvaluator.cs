using RiskFirst.Hateoas.Models;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public interface ILinksEvaluator
    {
        void BuildLinks(IEnumerable<ILinkSpec> links, ILinkContainer container);
    }
}
