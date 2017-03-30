using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public class DefaultLinkTransformationContextFactory : ILinkTransformationContextFactory
    {
        private readonly IActionContextAccessor actionAccessor;
        private readonly ILoggerFactory loggerFactory;
        public DefaultLinkTransformationContextFactory(IActionContextAccessor actionAccessor, ILoggerFactory loggerFactory)
        {
            this.actionAccessor = actionAccessor;
            this.loggerFactory = loggerFactory;
        }
        public LinkTransformationContext CreateContext(ILinkSpec spec)
        {
            return new LinkTransformationContext(spec,actionAccessor.ActionContext);
        }
    }
}
