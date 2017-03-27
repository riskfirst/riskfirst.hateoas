using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public class DefaultLinkAuthorizationService : ILinkAuthorizationService
    {
        private readonly IAuthorizationService authService;
        public DefaultLinkAuthorizationService(IAuthorizationService authService)
        {
            this.authService = authService;
        }

        public async Task<bool> AuthorizeLink<TResource>(LinkAuthorizationContext<TResource> context)
        {
            if (!context.User.Identity.IsAuthenticated)
                return false;

            if (context.ShouldAuthorizeRoute)
            {
                var authAttrs = GetRouteAuthorizationInfo(context.RouteInfo);
                foreach (var authAttr in authAttrs)
                {
                    if (!await AuthorizeData(authAttr, context.User, context.RouteValues))
                        return false;
                    return true;
                }
            }

            foreach (var policy in context.AuthorizePolicies)
            {
                if (!await AuthorizePolicy(policy, context.User, context.Resource))
                    return false;
            }
            return true;
        }

        protected virtual async Task<bool> AuthorizeData(IAuthorizeData authData, ClaimsPrincipal user, RouteValueDictionary values)
        {
            if (!user.Identity.IsAuthenticated)
                return false;

            if (!String.IsNullOrEmpty(authData.Roles))
            {
                if (!authData.Roles.Split(',').Any(r => user.IsInRole(r)))
                {
                    return false;
                }
            }
            if (!String.IsNullOrEmpty(authData.Policy) && !await authService.AuthorizeAsync(user, values, authData.Policy))
            {
                return false;
            }
            return true;
        }

        protected virtual async Task<bool> AuthorizePolicy<TResource>(string policy, ClaimsPrincipal user, TResource resource)
        {
            return await authService.AuthorizeAsync(user, resource, policy);
        }

        protected virtual IEnumerable<IAuthorizeData> GetRouteAuthorizationInfo(RouteInfo routeInfo)
        {
            return routeInfo.MethodInfo.GetCustomAttributes<AuthorizeAttribute>()
                              .Union(routeInfo.ControllerType.GetTypeInfo().GetCustomAttributes<AuthorizeAttribute>());
        }



    }
}
