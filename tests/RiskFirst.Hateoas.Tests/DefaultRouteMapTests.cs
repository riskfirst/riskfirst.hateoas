using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using RiskFirst.Hateoas.Polyfills;
using RiskFirst.Hateoas.Tests.Controllers;
using RiskFirst.Hateoas.Tests.Infrastructure;
using Xunit;

namespace RiskFirst.Hateoas.Tests
{
    [Trait("Category", "RouteMap")]
    public class DefaultRouteMapTests
    {
        [AutonamedFact]
        public void GivenAssemblyLoaderProvidesControllerAssemblies_RoutesAreRegistered()
        {
            var contextAccessorMock = new Mock<IActionContextAccessor>();
            var loggerMock = new Mock<ILogger<DefaultRouteMap>>();

            var routeMap = new DefaultRouteMap(contextAccessorMock.Object, loggerMock.Object, new DefaultAssemblyLoader());
            var route = routeMap.GetRoute(MvcController.GetAllValuesRoute);
            Assert.NotNull(route);
        }


        [AutonamedFact]
        public void GivenAssemblyLoaderProvidesApiControllerAssemblies_RoutesAreRegistered()
        {
            var contextAccessorMock = new Mock<IActionContextAccessor>();
            var loggerMock = new Mock<ILogger<DefaultRouteMap>>();

            var routeMap = new DefaultRouteMap(contextAccessorMock.Object, loggerMock.Object, new DefaultAssemblyLoader());
            var route = routeMap.GetRoute(ApiController.GetAllValuesRoute);
            Assert.NotNull(route);
        }
    }
}
