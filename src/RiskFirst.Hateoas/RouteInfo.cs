using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace RiskFirst.Hateoas
{
    public class RouteInfo
    {
        public RouteInfo(string name, HttpMethod method, MethodInfo methodInfo, Type controllerType)
        {
            this.RouteName = name;
            this.HttpMethod = method;
            this.MethodInfo = methodInfo;
            this.ControllerType = controllerType;
        }
        public string RouteName { get; }
        public HttpMethod HttpMethod { get; }
        public MethodInfo MethodInfo { get; }
        public Type ControllerType { get; }

        public string ControllerName => ControllerType.Name.Replace("Controller", String.Empty);
        public IEnumerable<LinksAttribute> LinksAttributes => MethodInfo.GetCustomAttributes<LinksAttribute>();

    }
}
