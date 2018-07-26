using System.Collections.Generic;

namespace RiskFirst.Hateoas.Models
{
    public interface ILinkContainer
    {
        Dictionary<string, Link> Links { get; set; }
        void AddLink(string id, Link link);
    }
}
