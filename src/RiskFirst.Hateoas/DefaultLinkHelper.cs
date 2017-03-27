using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskFirst.Hateoas
{
    public class DefaultLinkHelper : ILinkHelper
    {
        private StringBuilder stringBuilder;
        private readonly RouteValueDictionary routeValueDictionary;

        public DefaultLinkHelper(IActionContextAccessor accessor)
        {
            ActionContext = accessor.ActionContext;
            routeValueDictionary = new RouteValueDictionary();
        }
        public ActionContext ActionContext { get; }

        protected RouteValueDictionary AmbientValues => ActionContext.RouteData.Values;
        protected HttpContext HttpContext => ActionContext.HttpContext;
        protected IRouter Router => ActionContext.RouteData.Routers[0];


        public virtual string GetHref(LinkSpec link)
        {
            if (String.IsNullOrEmpty(link.RouteName))
                throw new ArgumentNullException(nameof(link.RouteName));

            var valuesDictionary = link.Values as RouteValueDictionary ?? GetValuesDictionary(link.Values);
            var virtualPathData = GetVirtualPathData(link.RouteName, valuesDictionary);
            return GenerateUrl(virtualPathData, String.Empty);
        }

        public virtual string GetRel(LinkSpec link)
        {
            return link.ControllerName + "/" + link.RouteName;
        }
        protected virtual VirtualPathData GetVirtualPathData(string routeName, RouteValueDictionary values)
        {
            var context = new VirtualPathContext(HttpContext, AmbientValues, values, routeName);
            return Router.GetVirtualPath(context);
        }

        protected string GenerateUrl(string virtualPath, string query, string fragment)
        {
            if (String.IsNullOrEmpty(virtualPath))
            {
                throw new ArgumentNullException(nameof(virtualPath));
            }
            var protocol = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host.ToUriComponent();
            var builder = GetStringBuilder();
            try
            {
                builder.Append(protocol);
                builder.Append("://");
                builder.Append(host);
                if (!virtualPath.StartsWith("/", StringComparison.Ordinal))
                {
                    builder.Append("/");
                }
                builder.Append(virtualPath);
                if(!string.IsNullOrEmpty(query))
                {
                    builder.Append("?").Append(query);
                }
                if (!string.IsNullOrEmpty(fragment))
                {
                    builder.Append("#").Append(fragment);
                }
                var path = builder.ToString();
                return path;
            }
            finally
            {
                builder.Clear();
            }
        }
        protected string GenerateUrl(VirtualPathData pathData, string fragment)
        {
            if (pathData == null)
            {
                throw new ArgumentNullException(nameof(pathData));
            }
            var protocol = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host.ToUriComponent();
            var pathBase = HttpContext.Request.PathBase;

            var builder = GetStringBuilder();
            try
            {
                builder.Append(protocol);
                builder.Append("://");
                builder.Append(host);
                if (!pathBase.HasValue)
                {
                    if (pathData.VirtualPath.Length == 0)
                    {
                        builder.Append("/");
                    }
                    else
                    {
                        if (!pathData.VirtualPath.StartsWith("/", StringComparison.Ordinal))
                        {
                            builder.Append("/");
                        }

                        builder.Append(pathData.VirtualPath);
                    }
                }
                else
                {
                    if (pathData.VirtualPath.Length == 0)
                    {
                        builder.Append(pathBase.Value);
                    }
                    else
                    {
                        builder.Append(pathBase.Value);

                        if (pathBase.Value.EndsWith("/", StringComparison.Ordinal))
                        {
                            builder.Length--;
                        }

                        if (!pathData.VirtualPath.StartsWith("/", StringComparison.Ordinal))
                        {
                            builder.Append("/");
                        }

                        builder.Append(pathData.VirtualPath);
                    }
                }
                if (!string.IsNullOrEmpty(fragment))
                {
                    builder.Append("#").Append(fragment);
                }
                var path = builder.ToString();
                return path;
            }
            finally
            {
                // Clear the StringBuilder so that it can reused for the next call.
                builder.Clear();
            }
        }
        private StringBuilder GetStringBuilder()
        {
            if (stringBuilder == null)
            {
                stringBuilder = new StringBuilder();
            }

            return stringBuilder;
        }
        protected RouteValueDictionary GetValuesDictionary(object values)
        {
            var routeValuesDictionary = values as RouteValueDictionary;
            if (routeValuesDictionary != null)
            {
                routeValueDictionary.Clear();
                foreach (var kvp in routeValuesDictionary)
                {
                    routeValueDictionary.Add(kvp.Key, kvp.Value);
                }

                return routeValueDictionary;
            }

            var dictionaryValues = values as IDictionary<string, object>;
            if (dictionaryValues != null)
            {
                routeValueDictionary.Clear();
                foreach (var kvp in dictionaryValues)
                {
                    routeValueDictionary.Add(kvp.Key, kvp.Value);
                }

                return routeValueDictionary;
            }

            return new RouteValueDictionary(values);
        }


    }
}
