using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace RiskFirst.Hateoas
{
    public class RouteInfo
    {
        public RouteInfo(string name, HttpMethod httpMethod, MethodInfo methodInfo)
        {
            this.RouteName = name ?? $"{methodInfo.DeclaringType.Namespace}.{methodInfo.DeclaringType.Name}.{ methodInfo.Name}";
            this.HttpMethod = httpMethod;
            this.MethodInfo = methodInfo;
        }
        public string RouteName { get; }
        public HttpMethod HttpMethod { get; }
        public MethodInfo MethodInfo { get; }
        public Type ControllerType => MethodInfo.DeclaringType;
        public string MethodName => MethodInfo?.Name;
        public string ControllerName => ControllerType?.Name?.Replace("Controller", String.Empty);
        public IEnumerable<LinksAttribute> LinksAttributes => MethodInfo.GetCustomAttributes<LinksAttribute>();

    }
}
