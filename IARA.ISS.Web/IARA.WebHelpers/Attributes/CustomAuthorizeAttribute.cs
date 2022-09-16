using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IARA.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.WebHelpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public CustomAuthorizeAttribute(params string[] permissions)
        {
            this.permissions = permissions;
        }

        private string[] permissions;

        public string Permissions
        {
            get
            {
                return permissions != null ? string.Join(",", permissions) : null;
            }
            set
            {
                permissions = value != null ? value.Split(",") : null;
            }
        }

        private PermissionsCache GetPermissions(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<PermissionsCache>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            PermissionsCache permissionsCache = GetPermissions(context.HttpContext.RequestServices);

            switch (AuthorizeCore(context.HttpContext.User, permissionsCache))
            {
                case AuthorizeResultCodes.UNAUTHORIZED:
                    context.Result = new UnauthorizedObjectResult("unauthorized access");
                    break;

                case AuthorizeResultCodes.FORBIDDEN:
                    context.Result = new ForbidResult();
                    break;

                case AuthorizeResultCodes.ALLOW:
                    //ALLOW User to continue execution
                    break;

                default:
                    throw new NotImplementedException();
            }
            return;
        }

        private AuthorizeResultCodes AuthorizeCore(ClaimsPrincipal claimsPrincipal, PermissionsCache permissionsCache)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(Permissions))
                {
                    List<int> userPermissionIds = claimsPrincipal.GetPermissionIds();

                    List<int> methodPermissionIds = permissions.Select(x => permissionsCache.GetPermissionIdByCode(x)).ToList();

                    return userPermissionIds != null && methodPermissionIds.Intersect(userPermissionIds).Any()
                        ? AuthorizeResultCodes.ALLOW
                        : AuthorizeResultCodes.FORBIDDEN;
                }
                else
                {
                    return AuthorizeResultCodes.ALLOW;
                }
            }
            else
            {
                return AuthorizeResultCodes.UNAUTHORIZED;
            }
        }

        private enum AuthorizeResultCodes
        {
            UNAUTHORIZED,
            FORBIDDEN,
            ALLOW
        }
    }
}
