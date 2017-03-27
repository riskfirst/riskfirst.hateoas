using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public interface ILinksPolicy
    {

    }
    public interface ILinksPolicy<TResource> : ILinksPolicy
    {
        IReadOnlyList<ILinksRequirement<TResource>> Requirements { get; }
    }
}
