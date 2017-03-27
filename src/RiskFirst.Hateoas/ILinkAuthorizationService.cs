using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public interface ILinkAuthorizationService
    {
        Task<bool> AuthorizeLink<TResource>(LinkAuthorizationContext<TResource> context);
    }
}
