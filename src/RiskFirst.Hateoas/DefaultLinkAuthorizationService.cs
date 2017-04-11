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
            if (authService == null)
                throw new ArgumentNullException(nameof(authService));
            this.authService = authService;
        }

        public async Task<bool> AuthorizeLink<TResource>(LinkAuthorizationContext<TResource> context)
        {
            if (!(context.User?.Identity?.IsAuthenticated ?? false))
                return false;

            if (context.ShouldAuthorizeRoute)
            {
                var authAttrs = context.RouteInfo.AuthorizeAttributes;
                foreach (var authAttr in authAttrs)
                {
                    if (!await AuthorizeData(authAttr, context.User, context.RouteValues))
                    {
                        return false;
                    }
                }
            }
            if (context.AuthorizationRequirements.Any())
            {
                if (!await authService.AuthorizeAsync(context.User, context.Resource, context.AuthorizationRequirements))
                {
                    return false;
                }
            }
            if(context.AuthorizationPolicyNames.Any())
            {
                var tasks = context.AuthorizationPolicyNames.Select(policyName => authService.AuthorizeAsync(context.User, context.Resource, policyName)).ToList();
                await Task.WhenAll(tasks);
               
                if(!tasks.All(x => x.Result))
                {
                    return false;
                }
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
    }
}
