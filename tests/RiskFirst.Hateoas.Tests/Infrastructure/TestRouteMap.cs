using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Tests.Infrastructure
{
    public class TestRouteMap : IRouteMap
    {
        private IDictionary<string, RouteInfo> routes = new Dictionary<string, RouteInfo>();
        private string currentRoute;

        public TestRouteMap AddRoute(RouteInfo routeInfo)
        {
            this.routes.Add(routeInfo.RouteName, routeInfo);
            return this;
        }

        public TestRouteMap SetCurrentRoute(string name)
        {
            if (routes.ContainsKey(name))
                this.currentRoute = name;
            return this;
        }

        public RouteInfo GetCurrentRoute()
        {
            return routes[this.currentRoute];
        }

        public RouteInfo GetRoute(string name)
        {
            return routes.ContainsKey(name)? routes[name] : null;
        }
    }
}
