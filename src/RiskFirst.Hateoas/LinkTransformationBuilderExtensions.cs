using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public static class LinkTransformationBuilderExtensions
    {
        public static LinkTransformationBuilder AddProtocol(this LinkTransformationBuilder builder, string scheme = null)
        {
            return builder.Add(ctx => String.Concat(scheme ?? ctx.HttpContext.Request.Scheme, "://"));
        }

        public static LinkTransformationBuilder AddHost(this LinkTransformationBuilder builder)
        {
            return builder.Add(ctx => ctx.HttpContext.Request.Host.ToUriComponent());
        }

        public static LinkTransformationBuilder AddRoutePath(this LinkTransformationBuilder builder)
        {
            return builder.Add(ctx =>
            {
                if (string.IsNullOrEmpty(ctx.LinkSpec.RouteName))
                    throw new InvalidOperationException($"Invalid route specified in link specification.");

                var path = ctx.LinkGenerator.GetPathByRouteValues(ctx.HttpContext, ctx.LinkSpec.RouteName, ctx.LinkSpec.RouteValues);

                if (string.IsNullOrEmpty(path))
                    throw new InvalidOperationException($"Invalid path when adding route '{ctx.LinkSpec.RouteName}'. RouteValues: {string.Join(",", ctx.ActionContext.RouteData.Values.Select(x => string.Concat(x.Key, "=", x.Value)))}");

                return path;
            });
        }
        public static LinkTransformationBuilder AddVirtualPath(this LinkTransformationBuilder builder,string path)
        {
            return builder.AddVirtualPath(ctx => path);
        }
        public static LinkTransformationBuilder AddVirtualPath(this LinkTransformationBuilder builder,Func<LinkTransformationContext, string> getPath)
        {
            return builder.Add(ctx =>
            {
                var path = getPath(ctx);
                if (!path.StartsWith("/"))
                    path = String.Concat("/", path);
                return path;
            });
        }
        public static LinkTransformationBuilder AddQueryStringValues(this LinkTransformationBuilder builder, IDictionary<string, string> values)
        {
            return builder.Add(ctx =>
            {
                var queryString = String.Join("&", values.Select(v => $"{v.Key}={v.Value?.ToString()}"));
                return string.Concat("?", queryString);
            });
        }
        public static LinkTransformationBuilder AddFragment(this LinkTransformationBuilder builder, Func<LinkTransformationContext, string> getFragment)
        {
            return builder.Add(ctx => string.Concat("#", getFragment(ctx)));
        }

        private static RouteValueDictionary GetValuesDictionary(object values)
        {
            var routeValuesDictionary = values as RouteValueDictionary;
            if (routeValuesDictionary != null)
            {
                return routeValuesDictionary;
            }

            var dictionaryValues = values as IDictionary<string, object>;
            if (dictionaryValues != null)
            {
                routeValuesDictionary = new RouteValueDictionary();
                foreach (var kvp in dictionaryValues)
                {
                    routeValuesDictionary.Add(kvp.Key, kvp.Value);
                }

                return routeValuesDictionary;
            }

            return new RouteValueDictionary(values);
        }
    }
}
