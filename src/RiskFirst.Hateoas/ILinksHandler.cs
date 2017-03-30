using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public interface ILinksHandler
    {
        Task HandleAsync<T>(LinksHandlerContext<T> context);
    }
}
