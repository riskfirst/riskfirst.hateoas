using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Moq;
using RiskFirst.Hateoas.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace RiskFirst.Hateoas.Tests
{
    [Trait("Category","Authorization")]
    public class DefaultLinkAuthorizationServiceTests
    {
        private LinksAuthorizationTestCase ConfigureTestCase(Action<TestCaseBuilder> configureTest = null)
        {
            var builder = new TestCaseBuilder();
            configureTest?.Invoke(builder);
            return builder.BuildAuthorizationServiceTestCase();
        }

        [AutonamedFact]
        public async Task AuthService_GivenUnauthenticatedUser_DoesNotAuthorize()
        {
            var testCase = ConfigureTestCase(builder =>
            {
                builder.WithIdentity(user =>
                {
                    user.SetupGet(x => x.IsAuthenticated)
                            .Returns(false);
                })
                .WithMockAuthorizationService()
                .WithMockRouteMap(routeMap =>
                {
                    routeMap.Setup(x => x.GetRoute(It.IsAny<string>()))
                            .Returns<string>(routeName => new RouteInfo(routeName, HttpMethod.Get, new Mock<IControllerMethodInfo>().Object));
                }); 
            });

            var model = new TestLinkContainer();
            var context = testCase.CreateContext(model, true);
            var result = await testCase.UnderTest.AuthorizeLink(context);

            Assert.False(result, "Expected authorization deny");
        }

        [AutonamedFact]
        public async Task AuthService_GivenRouteAuthorizationWithRole_DoesNotAuthorize()
        {
            var testCase = ConfigureTestCase(builder =>
            {
                builder.WithIdentity(user =>
                {
                    user.SetupGet(x => x.IsAuthenticated)
                            .Returns(true);
                })
                .WithMockAuthorizationService()
                .WithMockRouteMap(routeMap =>
                {
                    var methodInfoMock = new Mock<IControllerMethodInfo>();
                    methodInfoMock.Setup(x => x.GetAttributes<AuthorizeAttribute>())
                                    .Returns(new[] { new AuthorizeAttribute() { Roles = "Role1" } });
                    routeMap.Setup(x => x.GetRoute(It.IsAny<string>()))
                            .Returns<string>(routeName => new RouteInfo(routeName, HttpMethod.Get, methodInfoMock.Object));
                });
            });

            var model = new TestLinkContainer();
            var context = testCase.CreateContext(model, true);
            var result = await testCase.UnderTest.AuthorizeLink(context);

            Assert.False(result, "Expected authorization deny");
        }

        [AutonamedFact]
        public async Task AuthService_GivenRouteAuthorizationWithRole_DoesAuthorize()
        {
            var testCase = ConfigureTestCase(builder =>
            {
                builder.WithIdentity(user =>
                {
                    user.SetupGet(x => x.IsAuthenticated)
                            .Returns(true);
                })
                .WithPrincipal(principal =>
                {
                    principal.Setup(x => x.IsInRole("Role1"))
                                .Returns(true);
                })
                .WithMockAuthorizationService()
                .WithMockRouteMap(routeMap =>
                {
                    var methodInfoMock = new Mock<IControllerMethodInfo>();
                    methodInfoMock.Setup(x => x.GetAttributes<AuthorizeAttribute>())
                                    .Returns(new[] { new AuthorizeAttribute() { Roles = "Role1" } });
                    routeMap.Setup(x => x.GetRoute(It.IsAny<string>()))
                            .Returns<string>(routeName => new RouteInfo(routeName, HttpMethod.Get, methodInfoMock.Object));
                });
            });

            var model = new TestLinkContainer();
            var context = testCase.CreateContext(model, true);
            var result = await testCase.UnderTest.AuthorizeLink(context);

            Assert.True(result, "Expected authorization grant");
        }

        [AutonamedFact]
        public async Task AuthService_GivenRouteAuthorizationWithPolicy_DoesNotAuthorize()
        {
            var testCase = ConfigureTestCase(builder =>
            {
                builder.WithIdentity(user =>
                {
                    user.SetupGet(x => x.IsAuthenticated)
                            .Returns(true);
                })
                .WithMockAuthorizationService(authSvc =>
                {
                    authSvc.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<RouteValueDictionary>(), "Policy1"))
                                .Returns(Task.FromResult(false));
                })
                .WithMockRouteMap(routeMap =>
                {
                    var methodInfoMock = new Mock<IControllerMethodInfo>();
                    methodInfoMock.Setup(x => x.GetAttributes<AuthorizeAttribute>())
                                    .Returns(new[] { new AuthorizeAttribute("Policy1") });
                    routeMap.Setup(x => x.GetRoute(It.IsAny<string>()))
                            .Returns<string>(routeName => new RouteInfo(routeName, HttpMethod.Get, methodInfoMock.Object));
                });
            });

            var model = new TestLinkContainer();
            var context = testCase.CreateContext(model, true);
            var result = await testCase.UnderTest.AuthorizeLink(context);

            Assert.False(result, "Expected authorization deny");
        }

        [AutonamedFact]
        public async Task AuthService_GivenRouteAuthorizationWithPolicy_DoesAuthorize()
        {
            var testCase = ConfigureTestCase(builder =>
            {
                builder.WithIdentity(user =>
                {
                    user.SetupGet(x => x.IsAuthenticated)
                            .Returns(true);
                })
                .WithMockAuthorizationService(authSvc =>
                {
                    authSvc.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<RouteValueDictionary>(), "Policy1"))
                                .Returns(Task.FromResult(true));
                })
                .WithMockRouteMap(routeMap =>
                {
                    var methodInfoMock = new Mock<IControllerMethodInfo>();
                    methodInfoMock.Setup(x => x.GetAttributes<AuthorizeAttribute>())
                                    .Returns(new[] { new AuthorizeAttribute("Policy1") });
                    routeMap.Setup(x => x.GetRoute(It.IsAny<string>()))
                            .Returns<string>(routeName => new RouteInfo(routeName, HttpMethod.Get, methodInfoMock.Object));
                });
            });

            var model = new TestLinkContainer();
            var context = testCase.CreateContext(model, true);
            var result = await testCase.UnderTest.AuthorizeLink(context);

            Assert.True(result, "Expected authorization grant");
        }

        [AutonamedFact]
        public async Task AuthService_GivenRequirements_DoesNotAuthorize()
        {
            var testCase = ConfigureTestCase(builder =>
            {
                builder.WithIdentity(user =>
                {
                    user.SetupGet(x => x.IsAuthenticated)
                            .Returns(true);
                })
                .WithMockAuthorizationService(authSvc =>
                {
                    authSvc.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestLinkContainer>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                                .Returns(Task.FromResult(false));
                })
                .WithMockRouteMap(routeMap =>
                {
                    var methodInfoMock = new Mock<IControllerMethodInfo>();                   
                    routeMap.Setup(x => x.GetRoute(It.IsAny<string>()))
                            .Returns<string>(routeName => new RouteInfo(routeName, HttpMethod.Get, methodInfoMock.Object));
                });
            });

            var model = new TestLinkContainer();
            var context = testCase.CreateContext(model, false,new[] { new DummyAuthorizationRequirement() } );
            var result = await testCase.UnderTest.AuthorizeLink(context);

            Assert.False(result, "Expected authorization deny");
        }

        [AutonamedFact]
        public async Task AuthService_GivenRequirements_DoesAuthorize()
        {
            var testCase = ConfigureTestCase(builder =>
            {
                builder.WithIdentity(user =>
                {
                    user.SetupGet(x => x.IsAuthenticated)
                            .Returns(true);
                })
                .WithMockAuthorizationService(authSvc =>
                {
                    authSvc.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestLinkContainer>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                                .Returns(Task.FromResult(true));
                })
                .WithMockRouteMap(routeMap =>
                {
                    var methodInfoMock = new Mock<IControllerMethodInfo>();
                    routeMap.Setup(x => x.GetRoute(It.IsAny<string>()))
                            .Returns<string>(routeName => new RouteInfo(routeName, HttpMethod.Get, methodInfoMock.Object));
                });
            });

            var model = new TestLinkContainer();
            var context = testCase.CreateContext(model, false, new[] { new DummyAuthorizationRequirement() });
            var result = await testCase.UnderTest.AuthorizeLink(context);

            Assert.True(result, "Expected authorization grant");
        }

        [AutonamedFact]
        public async Task AuthService_GivenNamedPolicy_DoesNotAuthorize()
        {
            var testCase = ConfigureTestCase(builder =>
            {
                builder.WithIdentity(user =>
                {
                    user.SetupGet(x => x.IsAuthenticated)
                            .Returns(true);
                })
                .WithMockAuthorizationService(authSvc =>
                {
                    authSvc.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestLinkContainer>(), It.IsAny<string>()))
                                .Returns(Task.FromResult(false));
                })
                .WithMockRouteMap(routeMap =>
                {
                    var methodInfoMock = new Mock<IControllerMethodInfo>();
                    routeMap.Setup(x => x.GetRoute(It.IsAny<string>()))
                            .Returns<string>(routeName => new RouteInfo(routeName, HttpMethod.Get, methodInfoMock.Object));
                });
            });

            var model = new TestLinkContainer();
            var context = testCase.CreateContext(model, false, null, new[] { "Policy1" });
            var result = await testCase.UnderTest.AuthorizeLink(context);

            Assert.False(result, "Expected authorization deny");
        }

        [AutonamedFact]
        public async Task AuthService_GivenNamedPolicy_DoesAuthorize()
        {
            var testCase = ConfigureTestCase(builder =>
            {
                builder.WithIdentity(user =>
                {
                    user.SetupGet(x => x.IsAuthenticated)
                            .Returns(true);
                })
                .WithMockAuthorizationService(authSvc =>
                {
                    authSvc.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestLinkContainer>(), It.IsAny<string>()))
                                .Returns(Task.FromResult(true));
                })
                .WithMockRouteMap(routeMap =>
                {
                    var methodInfoMock = new Mock<IControllerMethodInfo>();
                    routeMap.Setup(x => x.GetRoute(It.IsAny<string>()))
                            .Returns<string>(routeName => new RouteInfo(routeName, HttpMethod.Get, methodInfoMock.Object));
                });
            });

            var model = new TestLinkContainer();
            var context = testCase.CreateContext(model, false, null, new[] { "Policy1" });
            var result = await testCase.UnderTest.AuthorizeLink(context);

            Assert.True(result, "Expected authorization grant");
        }

        private class DummyAuthorizationRequirement : IAuthorizationRequirement
        {

        }
    }
}
