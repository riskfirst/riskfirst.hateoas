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
        public LinkTransformationContext(ILinkSpec spec, ActionContext actionContext, LinkGenerator linkGenerator)
        {
            this.LinkSpec = spec;
            this.ActionContext = actionContext;
            this.LinkGenerator = linkGenerator;
        }
        public virtual ILinkSpec LinkSpec { get; }
        public ActionContext ActionContext { get; }
        public HttpContext HttpContext => ActionContext.HttpContext;
        public RouteValueDictionary RouteValues => ActionContext.RouteData.Values;
        public LinkGenerator LinkGenerator {get;}
    }
}
