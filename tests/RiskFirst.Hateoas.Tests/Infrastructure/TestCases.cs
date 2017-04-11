using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Tests.Infrastructure
{
    public class LinksServiceTestCase
    {
        public ILinksService UnderTest { get; }
        public TestLinksHandlerContextFactory LinksHandlerContextFactory { get; }
        public Mock<ILinkAuthorizationService> AuthServiceMock { get; }
        public Mock<ILogger<DefaultLinksService>> ServiceLoggerMock { get; }
        public LinksServiceTestCase(ILinksService underTest, 
            TestLinksHandlerContextFactory linksHandlerContextFactory, 
            Mock<ILinkAuthorizationService> authServiceMock,
            Mock<ILogger<DefaultLinksService>> serviceLoggerMock)
        {
            this.UnderTest = underTest;
            this.LinksHandlerContextFactory = linksHandlerContextFactory;
            this.AuthServiceMock = authServiceMock;
            this.ServiceLoggerMock = serviceLoggerMock;
        }
    }

    public class LinksEvaluatorTestCase
    {
        public ILinksEvaluator UnderTest { get; }
        public Mock<ILinkTransformation> HrefTransformMock { get; }
        public Mock<ILinkTransformation> RelTransformMock { get; }
        public LinksEvaluatorTestCase(ILinksEvaluator underTest,
                Mock<ILinkTransformation> hrefTransformMock, Mock<ILinkTransformation> relTransformMock)
        {
            this.UnderTest = underTest;
            this.HrefTransformMock = hrefTransformMock;
            this.RelTransformMock = relTransformMock;
        }
    }

    public class LinksAuthorizationTestCase
    {
        public ILinkAuthorizationService UnderTest { get; }

        public ClaimsPrincipal User { get; }

        public Mock<IAuthorizationService> AuthorizationServiceMock { get; }

        private IRouteMap routeMap;
        private bool requiresRouteAuthorization;
        private IEnumerable<IAuthorizationRequirement> requirements;
        private IEnumerable<string> policies;

        public LinksAuthorizationTestCase(ILinkAuthorizationService underTest, ClaimsPrincipal user, IRouteMap routeMap, Mock<IAuthorizationService> authServiceMock)
        {
            this.UnderTest = underTest;
            this.User = user;
            this.routeMap = routeMap;
            this.AuthorizationServiceMock = authServiceMock;
        }

        public LinkAuthorizationContext<TResource> CreateContext<TResource>(TResource resource, bool requiresRouteAuthorization, IEnumerable<IAuthorizationRequirement> requirements = null, IEnumerable<string> policies = null)
        {
            var route = this.routeMap.GetRoute("AuthTestRoute");
            return new LinkAuthorizationContext<TResource>(
                requiresRouteAuthorization,
                requirements,
                policies,
                route,
                new Microsoft.AspNetCore.Routing.RouteValueDictionary(),
                resource,
                this.User);
        }

    }
}
