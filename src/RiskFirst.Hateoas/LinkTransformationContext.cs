using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace RiskFirst.Hateoas
{
    public class LinkTransformationContext
    {
        public LinkTransformationContext(LinkSpec spec, ActionContext actionContext)
        {
            this.LinkSpec = spec;
            this.ActionContext = actionContext;
        }
        public LinkSpec LinkSpec { get; }
        public ActionContext ActionContext { get; }
        public HttpContext HttpContext => ActionContext.HttpContext;
        public RouteValueDictionary RouteValues => ActionContext.RouteData.Values;
        public IRouter Router => ActionContext.RouteData.Routers[0];
        
    }
}
