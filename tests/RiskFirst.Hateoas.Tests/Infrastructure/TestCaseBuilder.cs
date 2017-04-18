using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RiskFirst.Hateoas.Implementation;
using RiskFirst.Hateoas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Tests.Infrastructure
{
    public class TestCaseBuilder
    {
        private IRouteMap routeMap;
        private LinksOptions options = new LinksOptions();
        private List<ILinksHandler> handlers = new List<ILinksHandler>() { new PassThroughLinksHandler() };
       
        private Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
        private RouteData routeData = new RouteData();
        private Mock<ILinkTransformation> hrefTransformationMock;
        private Mock<ILinkTransformation> relTransformationMock;
        private Mock<ClaimsIdentity> identityMock;
        private Mock<ClaimsPrincipal> principalMock;
        private Mock<IAuthorizationService> authServiceMock;

        public TestCaseBuilder UseBasicTransformations()
        {
            var mockTransform = new Mock<ILinkTransformation>();
            mockTransform.Setup(x => x.Transform(It.IsAny<LinkTransformationContext>())).Returns<LinkTransformationContext>(FormatBasic);
            this.hrefTransformationMock = mockTransform;
            this.relTransformationMock = mockTransform;

            options.UseHrefTransformation(mockTransform.Object);
            options.UseRelTransformation(mockTransform.Object);
            return this;
        }

        public TestCaseBuilder UseMockHrefTransformation(Action<Mock<ILinkTransformation>> configureMock)
        {
            this.hrefTransformationMock = new Mock<ILinkTransformation>();
            configureMock?.Invoke(hrefTransformationMock);
            options.UseHrefTransformation(hrefTransformationMock.Object);
            return this;
        }

        public TestCaseBuilder UseLinkBuilderHrefTransformation(Action<LinkTransformationBuilder> configureBuilder)
        {
            var builder = new LinkTransformationBuilder();
            configureBuilder?.Invoke(builder);
            options.UseHrefTransformation(builder.Build());
            return this;
        }
        public TestCaseBuilder UseMockRelTransformation(Action<Mock<ILinkTransformation>> configureMock)
        {
            this.relTransformationMock = new Mock<ILinkTransformation>();
            configureMock?.Invoke(relTransformationMock);
            options.UseRelTransformation(relTransformationMock.Object);
            return this;
        }
        public TestCaseBuilder UseLinkBuilderRelTransformation(Action<LinkTransformationBuilder> configureBuilder)
        {
            var builder = new LinkTransformationBuilder();
            configureBuilder?.Invoke(builder);
            options.UseRelTransformation(builder.Build());
            return this;
        }
        public TestCaseBuilder WithTestRouteMap(Action<TestRouteMap> configureRouteMap = null)
        {
            var testRouteMap = new TestRouteMap();
            configureRouteMap?.Invoke(testRouteMap);
            this.routeMap = testRouteMap;
            return this;
        }

        public TestCaseBuilder WithMockRouteMap(Action<Mock<IRouteMap>> configureRouteMap = null)
        {
            var mockRouteMap = new Mock<IRouteMap>();
            configureRouteMap?.Invoke(mockRouteMap);
            this.routeMap = mockRouteMap.Object;
            return this;
        }

        public TestCaseBuilder WithMockAuthorizationService(Action<Mock<IAuthorizationService>> configureAuthService = null)
        {
            this.authServiceMock = new Mock<IAuthorizationService>();
            configureAuthService?.Invoke(authServiceMock);
            return this;
        }

        public TestCaseBuilder WithHandler<THandler>() where THandler : ILinksHandler, new()
        {
            this.handlers.Add(new THandler());
            return this;
        }

        public TestCaseBuilder WithIdentity(Action<Mock<ClaimsIdentity>> configureUser)
        {
            this.identityMock = new Mock<ClaimsIdentity>();
            configureUser?.Invoke(identityMock);
            return this;
        }

        public TestCaseBuilder WithPrincipal(Action<Mock<ClaimsPrincipal>> configurePrincipal)
        {
            this.principalMock = new Mock<ClaimsPrincipal>();
            configurePrincipal?.Invoke(principalMock);
            return this;
        }

        public TestCaseBuilder AddPolicy<TResource>(Action<LinksPolicyBuilder<TResource>> configurePolicy)
            where TResource : class
        {
            options.AddPolicy(configurePolicy);
            return this;
        }

        public TestCaseBuilder AddPolicy<TResource>(string name, Action<LinksPolicyBuilder<TResource>> configurePolicy)
            where TResource : class
        {
            options.AddPolicy(name, configurePolicy);
            return this;
        }

        public ILinksPolicy GetPolicy<TResource>()
        {
            return options.GetPolicy<TResource>();
        }

        public ILinksPolicy GetPolicy<TResource>(string name)
        {
            return options.GetPolicy<TResource>(name);
        }

        public TestCaseBuilder DefaultPolicy(Action<LinksPolicyBuilder<ILinkContainer>> configurePolicy)
        {
            var builder = new LinksPolicyBuilder<ILinkContainer>();
            configurePolicy?.Invoke(builder);
            options.DefaultPolicy = builder.Build();
            return this;
        }

        public TestCaseBuilder AddRouteValues(Action<RouteData> configureRouteData)
        {
            configureRouteData?.Invoke(this.routeData);
            return this;
        }


        public LinksServiceTestCase BuildLinksServiceTestCase()
        {
            var authServiceMock = new Mock<ILinkAuthorizationService>();
            var serviceLoggerMock = new Mock<ILogger<DefaultLinksService>>();
            var actionContext = CreateActionContext();
          
            var optionsWrapper = Options.Create(options);
            var policyProvider = new DefaultLinksPolicyProvider(optionsWrapper);
            var transformationContextFactory = new TestLinkTransformationContextFactory(actionContext);
            var linksHandlerFactory = new TestLinksHandlerContextFactory(routeMap, authServiceMock.Object, actionContext);
            var evaluator = new DefaultLinksEvaluator(optionsWrapper, transformationContextFactory);

            var service = new DefaultLinksService(optionsWrapper, serviceLoggerMock.Object, linksHandlerFactory, policyProvider, handlers, routeMap, evaluator);
            return new LinksServiceTestCase(service, linksHandlerFactory, authServiceMock, serviceLoggerMock);
        }

        public LinksEvaluatorTestCase BuildLinksEvaluatorTestCase()
        {
            var actionContext = CreateActionContext();

            var optionsWrapper = Options.Create(options);
            var transformationContextFactory = new TestLinkTransformationContextFactory(actionContext);
            var evaluator = new DefaultLinksEvaluator(optionsWrapper, transformationContextFactory);

            return new LinksEvaluatorTestCase(evaluator, this.hrefTransformationMock, this.relTransformationMock);
        }

        public LinksAuthorizationTestCase BuildAuthorizationServiceTestCase()
        {
            var service = new DefaultLinkAuthorizationService(authServiceMock.Object);

            var claimsPrincipalMock = this.principalMock ?? new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.CallBase = true;
            claimsPrincipalMock.Object.AddIdentity(this.identityMock?.Object ?? new Mock<ClaimsIdentity>().Object);

            return new LinksAuthorizationTestCase(service, claimsPrincipalMock.Object, this.routeMap, this.authServiceMock);
        }

        private ActionContext CreateActionContext()
        {
            var actionContext = new ActionContext(httpContextMock.Object, routeData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            return actionContext;
        }

        private static string FormatBasic(LinkTransformationContext ctx)
        {
            var parameters = ctx.LinkSpec.RouteValues?.Select(x => $"{x.Key}={x.Value}");

            return ctx.LinkSpec.RouteName + (parameters != null && parameters.Any() ? String.Concat("?", String.Join("&", parameters)) : "");
        }

        private class TestLinkTransformationContextFactory : ILinkTransformationContextFactory
        {
            private readonly ActionContext actionContext;

            public TestLinkTransformationContextFactory(ActionContext actionContext)
            {
                this.actionContext = actionContext;
            }

            public LinkTransformationContext CreateContext(ILinkSpec spec)
            {
                return new LinkTransformationContext(spec, actionContext);
            }
        }
    }
}
