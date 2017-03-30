using Microsoft.AspNetCore.Routing;
using RiskFirst.Hateoas.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public static class LinksPolicyBuilderExtensions
    {
        public static LinksPolicyBuilder<TResource> RequireSelfLink<TResource>(this LinksPolicyBuilder<TResource> builder)
            where TResource : class
        {
            return builder.RequireSelfLink("self");
        }
        public static LinksPolicyBuilder<TResource> RequireSelfLink<TResource>(this LinksPolicyBuilder<TResource> builder,
            string id)
            where TResource : class
        {
            return builder.Requires(new SelfLinkRequirement<TResource>()
            {
                Id = id
            });
        }

        public static LinksPolicyBuilder<TResource> RequireRoutedLink<TResource>(this LinksPolicyBuilder<TResource> builder,
            string id,
            string routeName)
            where TResource : class
        {
            return builder.RequireRoutedLink(id, routeName, null, condition: null);
        }

        public static LinksPolicyBuilder<TResource> RequireRoutedLink<TResource>(this LinksPolicyBuilder<TResource> builder,
            string id,
            string routeName,
            Func<TResource, object> getValues,
            Action<LinkConditionBuilder<TResource>> configureCondition)
            where TResource : class
        {
            var conditionBuilder = new LinkConditionBuilder<TResource>();
            configureCondition?.Invoke(conditionBuilder);
            return builder.RequireRoutedLink(id, routeName, getValues: getValues, condition: conditionBuilder.Build());
        }

        public static LinksPolicyBuilder<TResource> RequireRoutedLink<TResource>(this LinksPolicyBuilder<TResource> builder,
            string id,
            string routeName,
            Func<TResource, object> getValues,
            LinkCondition<TResource> condition = null)
            where TResource : class
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
            return builder.Requires(req);
        }
        public static LinksPolicyBuilder<TResource> RequiresPagingLinks<TResource>(this LinksPolicyBuilder<TResource> builder)
            where TResource : class
        {
            return builder.RequiresPagingLinks("currentPage", "nextPage", "previousPage");
        }
        public static LinksPolicyBuilder<TResource> RequiresPagingLinks<TResource>(this LinksPolicyBuilder<TResource> builder,
            string currentId,
            string nextId,
            string previousId)
            where TResource : class
        {
            return builder.RequiresPagingLinks(currentId, nextId, previousId, condition: null);
        }
        public static LinksPolicyBuilder<TResource> RequiresPagingLinks<TResource>(this LinksPolicyBuilder<TResource> builder,
            Action<LinkConditionBuilder<TResource>> configureCondition)
            where TResource : class
        {
            return builder.RequiresPagingLinks("currentPage", "nextPage", "previousPage");
        }
        public static LinksPolicyBuilder<TResource> RequiresPagingLinks<TResource>(this LinksPolicyBuilder<TResource> builder,
            string currentId,
            string nextId,
            string previousId,
            Action<LinkConditionBuilder<TResource>> configureCondition)
            where TResource : class
        {
            var conditionBuilder = new LinkConditionBuilder<TResource>();
            configureCondition?.Invoke(conditionBuilder);
            return builder. RequiresPagingLinks(currentId, nextId, previousId, condition: conditionBuilder.Build());
        }

        private static LinksPolicyBuilder<TResource> RequiresPagingLinks<TResource>(this LinksPolicyBuilder<TResource> builder,
            string currentId,
            string nextId,
            string previousId,
            LinkCondition<TResource> condition)
            where TResource : class
        {
            var req = new PagingLinksRequirement<TResource>()
            {
                CurrentId = currentId,
                NextId = nextId,
                PreviousId = previousId,
                Condition = condition ?? LinkCondition<TResource>.None
            };
            return builder.Requires(req);
        }
    }
}
