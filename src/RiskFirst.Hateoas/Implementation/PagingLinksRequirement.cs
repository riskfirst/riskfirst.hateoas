using Microsoft.AspNetCore.Routing;
using RiskFirst.Hateoas.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Implementation
{
    public class PagingLinksRequirement<TResource> : LinksHandler<PagingLinksRequirement<TResource>, TResource>, ILinksRequirement<TResource>
        where TResource : class
    {
        public const string QueryStringFormat = "?pagenumber={0}&pagesize={1}";

        public PagingLinksRequirement()
        {

        }
        public string CurrentId { get; set; }
        public string NextId { get; set; }
        public string PreviousId { get; set; }
        public LinkCondition<TResource> Condition { get; set; } = LinkCondition<TResource>.None;
        
        protected override async Task HandleRequirementAsync(LinksHandlerContext<TResource> context, PagingLinksRequirement<TResource> requirement)
        {
            var condition = requirement.Condition;
            if (condition != null && !condition.AssertAll(context.Resource))
            {
                context.Skipped(requirement, LinkRequirementSkipReason.Assertion);
                return;
            }

            var pagingResource = context.Resource as IPagedLinkContainer;
            if (pagingResource == null)
                throw new InvalidOperationException($"PagingLinkRequirement can only be used by a resource of type IPageLinkContainer. Type: {context.Resource.GetType().FullName}");

            var route = context.RouteMap.GetCurrentRoute();
            var values = context.CurrentRouteValues;

            if (condition != null && condition.RequiresAuthorization)
            {
                var authContext = new LinkAuthorizationContext<TResource>(
                    condition.RequiresRouteAuthorization,
                    condition.AuthorizationRequirements,
                    route,
                    values,
                    context.Resource,
                    context.User);

                if (!await context.Authorization.AuthorizeLink(authContext))
                {
                    context.Skipped(requirement, LinkRequirementSkipReason.Authorization);
                    return;
                }
            }

            context.Links.Add(new LinkSpec(requirement.CurrentId, route, GetPageValues(values, pagingResource.PageNumber, pagingResource.PageSize)));

            var addPrevLink = ShouldAddPreviousPageLink(pagingResource.PageNumber);
            var addNextLink = ShouldAddNextPageLink(pagingResource.PageNumber, pagingResource.PageCount);
            if (addPrevLink)
            {
                context.Links.Add(new LinkSpec(requirement.PreviousId, route, GetPageValues(values, pagingResource.PageNumber - 1, pagingResource.PageSize)));
            }
            if (addNextLink)
            {
                context.Links.Add(new LinkSpec(requirement.NextId, route, GetPageValues(values, pagingResource.PageNumber + 1, pagingResource.PageSize)));
            }
            context.Handled(requirement);
            return;
        }

        private RouteValueDictionary GetPageValues(object values, int pageNumber, int pageSize)
        {
            var newValues = new RouteValueDictionary(values);
            newValues.Add("pagenumber", pageNumber);
            newValues.Add("pagesize", pageSize);
            return newValues;
        }

        private bool ShouldAddPreviousPageLink(int pageNumber)
        {
            return pageNumber > 1;
        }

        private bool ShouldAddNextPageLink(int pageNumber, int pageCount)
        {
            return pageNumber < pageCount;
        }

    }
}
