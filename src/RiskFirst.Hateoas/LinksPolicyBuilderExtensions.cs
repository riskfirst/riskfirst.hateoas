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
        {
            return builder.RequireSelfLink("self");
        }
        public static LinksPolicyBuilder<TResource> RequireSelfLink<TResource>(this LinksPolicyBuilder<TResource> builder,
            string id)
        {
            return builder.Requires(new SelfLinkRequirement<TResource>()
            {
                Id = id
            });
        }

        public static LinksPolicyBuilder<TResource> RequireRoutedLink<TResource>(this LinksPolicyBuilder<TResource> builder,
            string id,
            string routeName)
        {
            return builder.RequireRoutedLink(id, routeName, null, condition: null);
        }

        public static LinksPolicyBuilder<TResource> RequireRoutedLink<TResource>(this LinksPolicyBuilder<TResource> builder,
            string id,
            string routeName,
            Func<TResource, object> getValues,
            Action<LinkConditionBuilder<TResource>> configureCondition)
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
        {
            return builder.RequiresPagingLinks("currentPage", "nextPage", "previousPage");
        }
        public static LinksPolicyBuilder<TResource> RequiresPagingLinks<TResource>(this LinksPolicyBuilder<TResource> builder,
            string currentId,
            string nextId,
            string previousId)
        {
            return builder.RequiresPagingLinks(currentId, nextId, previousId, condition: null);
        }
        public static LinksPolicyBuilder<TResource> RequiresPagingLinks<TResource>(this LinksPolicyBuilder<TResource> builder,
            Action<LinkConditionBuilder<TResource>> configureCondition)
        {
            return builder.RequiresPagingLinks("currentPage", "nextPage", "previousPage",configureCondition);
        }
        public static LinksPolicyBuilder<TResource> RequiresPagingLinks<TResource>(this LinksPolicyBuilder<TResource> builder,
            string currentId,
            string nextId,
            string previousId,
            Action<LinkConditionBuilder<TResource>> configureCondition)
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
