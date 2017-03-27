using Microsoft.AspNetCore.Routing;

namespace RiskFirst.Hateoas
{
    public interface IRouteMap
    {
        RouteValueDictionary GetCurrentRouteValues();
        RouteInfo GetCurrentRoute();
        RouteInfo GetRoute(string name);
    }
}
