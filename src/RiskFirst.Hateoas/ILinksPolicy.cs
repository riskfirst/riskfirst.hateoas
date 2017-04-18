using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public interface ILinksPolicy
    {
        IReadOnlyList<ILinksRequirement> Requirements { get; }
    }
}
