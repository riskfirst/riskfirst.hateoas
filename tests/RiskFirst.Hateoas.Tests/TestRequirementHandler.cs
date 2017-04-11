using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Tests
{
    public class TestRequirementHandler<TResource> : LinksHandler<TestRequirement<TResource>, TResource>
        where TResource : class
    {
        public TestRequirementHandler()
        {
        }
        protected override Task HandleRequirementAsync(LinksHandlerContext<TResource> context, TestRequirement<TResource> requirement)
        {
            var route = context.RouteMap.GetRoute("TestRoute");
            context.Links.Add(new LinkSpec("testLink",route,null));
            context.Handled(requirement);
            return Task.CompletedTask;
        }
    }
}
