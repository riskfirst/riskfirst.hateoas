using Microsoft.AspNetCore.Routing;
using RiskFirst.Hateoas.Implementation;
using System;
using System.Collections.Generic;

namespace RiskFirst.Hateoas
{
    public class LinksPolicyBuilder<TResource> where TResource : class
    {
        public LinksPolicyBuilder()
        {
        }
        public IList<ILinksRequirement<TResource>> Requirements { get; set; } = new List<ILinksRequirement<TResource>>();
        public LinksPolicyBuilder<TResource> Combine<T>(ILinksPolicy<T> policy) where T : class
        {
            foreach (var r in policy.Requirements)
                Requirements.Add(r.Convert<TResource>());
            return this;
        }
        public LinksPolicyBuilder<TResource> RequireSelfLink()
        {
            return this.RequireSelfLink("self");
        }
        public LinksPolicyBuilder<TResource> RequireSelfLink(
            string id)
        {
            this.Requirements.Add(new SelfLinkRequirement<TResource>()
            {
                Id = id
            });
            return this;
        }

        public LinksPolicyBuilder<TResource> RequireRoutedLink(
            string id,
            string routeName)
        {
            return RequireRoutedLink(id, routeName, null, condition: null);
        }

        public LinksPolicyBuilder<TResource> RequireRoutedLink(
            string id,
            string routeName,
            Func<TResource, object> getValues,
            Action<LinkConditionBuilder<TResource>> configureCondition)
        {
            var builder = new LinkConditionBuilder<TResource>();
            configureCondition?.Invoke(builder);
            return RequireRoutedLink(id, routeName, getValues: getValues, condition: builder.Build());
        }

        public LinksPolicyBuilder<TResource> RequireRoutedLink(
            string id,
            string routeName,
            Func<TResource, object> getValues,
            LinkCondition<TResource> condition = null)
        {
            Func<TResource, RouteValueDictionary> getRouteValues = r => new RouteValueDictionary();
            if (getValues != null)
                getRouteValues = r => new RouteValueDictionary(getValues(r));
            var req = new RouteLinkRequirement<TResource>()
            {
                Id = id,
                RouteName = routeName,
                GetRouteValues = getRouteValues,
                Condition = condition ?? LinkCondition<TResource>.None
            };
            this.Requirements.Add(req);
            return this;
        }
        public LinksPolicyBuilder<TResource> RequiresPagingLinks()
        {
            return RequiresPagingLinks("currentPage", "nextPage", "previousPage");
        }
        public LinksPolicyBuilder<TResource> RequiresPagingLinks(
            string currentId,
            string nextId,
            string previousId)
        {
            return RequiresPagingLinks(currentId, nextId, previousId, condition: null);
        }
        public LinksPolicyBuilder<TResource> RequiresPagingLinks(
            Action<LinkConditionBuilder<TResource>> configureCondition)
        {
            return RequiresPagingLinks("currentPage", "nextPage", "previousPage");
        }
        public LinksPolicyBuilder<TResource> RequiresPagingLinks(
            string currentId,
            string nextId,
            string previousId,
            Action<LinkConditionBuilder<TResource>> configureCondition)
        {
            var builder = new LinkConditionBuilder<TResource>();
            configureCondition?.Invoke(builder);
            return RequiresPagingLinks(currentId, nextId, previousId, condition: builder.Build());
        }

        private LinksPolicyBuilder<TResource> RequiresPagingLinks(
            string currentId,
            string nextId,
            string previousId,
            LinkCondition<TResource> condition)
        {
            var req = new PagingLinksRequirement<TResource>()
            {
                CurrentId = currentId,
                NextId = nextId,
                PreviousId = previousId,
                Condition = condition ?? LinkCondition<TResource>.None
            };
            this.Requirements.Add(req);
            return this;
        }

        public LinksPolicy<TResource> Build()
        {
            return new LinksPolicy<TResource>(Requirements);
        }

    }
}
