using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public class LinkTransformationBuilder
    {
        private IList<Func<LinkTransformationContext, string>> Transformations { get; } = new List<Func<LinkTransformationContext, string>>();
        public LinkTransformationBuilder AddProtocol()
        {
            Transformations.Add(ctx => String.Concat(ctx.HttpContext.Request.Scheme, "://"));
            return this;
        }

        public LinkTransformationBuilder AddHost()
        {
            Transformations.Add(ctx => ctx.HttpContext.Request.Host.ToUriComponent());
            return this;
        }

        public LinkTransformationBuilder AddRoutePath()
        {
            Transformations.Add(ctx =>
            {
                if (string.IsNullOrEmpty(ctx.LinkSpec.RouteName))
                    throw new InvalidOperationException($"Invalid route specified in link specification.");

                var valuesDictionary = GetValuesDictionary(ctx.LinkSpec.Values);
                var virtualPathContext = new VirtualPathContext(ctx.HttpContext, ctx.RouteValues, valuesDictionary, ctx.LinkSpec.RouteName);
                var virtualPathData = ctx.Router.GetVirtualPath(virtualPathContext);

                return virtualPathData.VirtualPath;
            });
            
            return this;
        }
        public LinkTransformationBuilder AddVirtualPath(string path)
        {
            AddVirtualPath(ctx => path);
            return this;
        }
        public LinkTransformationBuilder AddVirtualPath(Func<LinkTransformationContext,string> getPath)
        {
            Transformations.Add(ctx =>
            {
                var path = getPath(ctx);
                if (!path.StartsWith("/"))
                    path = String.Concat("/", path);
                return path;
            });
            return this;
        }
        public LinkTransformationBuilder AddQueryStringValues(IDictionary<string,string> values)
        {
            Transformations.Add(ctx =>
            {
                var queryString = String.Join("&", values.Select(v => $"{v.Key}={v.Value?.ToString()}"));
                return string.Concat("?", queryString);
            });
            return this;
        }
        public LinkTransformationBuilder AddFragment(Func<LinkTransformationContext,string> getFragment)
        {
            Transformations.Add(ctx => string.Concat("#", getFragment(ctx)));
            return this;
        }
        public LinkTransformationBuilder AddString(Func<LinkTransformationContext,string> getString)
        {
            Transformations.Add(getString);
            return this;
        }

        public ILinkTransformation Build()
        {
            return new BuilderLinkTransformation(Transformations);
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
