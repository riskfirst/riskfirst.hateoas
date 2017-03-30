using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Tests
{
    public class LinkTransformationTestContext : ILinkTransformationContextFactory
    {
        private readonly Mock<ActionContext> actionContext;
        public LinkTransformationTestContext()
        {
            actionContext = new Mock<ActionContext>();
        }

        public LinksOptions Options { get; } = new LinksOptions();        
        
        public LinkTransformationContext CreateContext(ILinkSpec spec)
        {
            return new LinkTransformationContext(spec, actionContext.Object);
        }
    }
}
