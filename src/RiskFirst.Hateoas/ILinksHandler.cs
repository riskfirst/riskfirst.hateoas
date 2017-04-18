using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public interface ILinksHandler
    {
        Task HandleAsync(LinksHandlerContext context);
    }
}
