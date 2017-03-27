using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public interface ILinksHandler
    {
        Task HandleAsync<T>(LinksHandlerContext<T> context);
    }
    public interface ILinksHandler<TResource> : ILinksHandler
    {
        Task HandleAsync(LinksHandlerContext<TResource> context);
    }
}
