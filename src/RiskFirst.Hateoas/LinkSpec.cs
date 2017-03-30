using Microsoft.AspNetCore.Routing;
using System;
using System.Net.Http;

namespace RiskFirst.Hateoas
{
    public interface ILinkSpec
    {
        string Id { get; }
        RouteInfo RouteInfo { get; }
        RouteValueDictionary RouteValues { get; }
        HttpMethod HttpMethod { get; }
        string RouteName { get; }
        string ControllerName { get; }
    }
    public struct LinkSpec : ILinkSpec
    {
        public LinkSpec(string id, RouteInfo routeInfo, RouteValueDictionary routeValues)
        {
            this.Id = id;
            this.RouteInfo = routeInfo;
            this.RouteValues = routeValues;
        }

        public string Id { get;  }
        public RouteInfo RouteInfo { get; }
        public RouteValueDictionary RouteValues { get; }
        public HttpMethod HttpMethod => RouteInfo?.HttpMethod;
        public string RouteName => RouteInfo?.RouteName;
        public string ControllerName => RouteInfo.ControllerName;
    }
}
