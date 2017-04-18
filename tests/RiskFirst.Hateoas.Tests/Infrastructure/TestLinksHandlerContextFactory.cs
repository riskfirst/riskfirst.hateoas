using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Tests.Infrastructure
{
    public class TestLinksHandlerContextFactory : ILinksHandlerContextFactory
    {
        private readonly IRouteMap routeMap;
        private readonly ILinkAuthorizationService authService;
        private readonly ActionContext actionContext;

        public TestLinksHandlerContextFactory(IRouteMap routeMap, ILinkAuthorizationService authService, ActionContext actionContext)
        {
            this.routeMap = routeMap;
            this.authService = authService;
            this.actionContext = actionContext;
        }

        public Mock LinksHandlerContextMock { get; private set; }
        public Mock LoggerMock { get; private set; }
        public LinksHandlerContext CreateContext(IEnumerable<ILinksRequirement> requirements, object resource)
        {
            LoggerMock = new Mock<ILogger<LinksHandlerContext>>();
            LinksHandlerContextMock = new Mock<LinksHandlerContext>(requirements, routeMap, authService, LoggerMock.Object, actionContext, resource);
            LinksHandlerContextMock.CallBase = true;
            return (LinksHandlerContext)LinksHandlerContextMock.Object;
        }

        public Mock<LinksHandlerContext> GetLinksHandlerContextMock<TResource>()
        {
            return (Mock<LinksHandlerContext>)this.LinksHandlerContextMock;
        }
    }
}
