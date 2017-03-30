using Microsoft.AspNetCore.Routing;

namespace RiskFirst.Hateoas
{
    public interface IRouteMap
    {
        RouteInfo GetCurrentRoute();
        RouteInfo GetRoute(string name);
    }
}
