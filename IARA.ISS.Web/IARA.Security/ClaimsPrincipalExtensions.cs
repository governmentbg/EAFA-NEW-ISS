using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using IARA.Common.Constants;
using IARA.Security.Enum;
using IdentityModel;

namespace IARA.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static List<int> GetPermissionIds(this ClaimsPrincipal claimsPrincipal)
        {
            string userPermissions = claimsPrincipal.Claims.Where(x => x.Type == CustomClaims.Permissions).Select(x => x.Value).FirstOrDefault();

            return !string.IsNullOrEmpty(userPermissions) ? userPermissions.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : null;
        }

        public static bool HasUserPermission(this IPrincipal user, string permissionCode)
        {
            return user is ClaimsPrincipal && HasUserPermission(user as ClaimsPrincipal, permissionCode);
        }

        public static bool HasUserPermission(this ClaimsPrincipal user, string permissionCode)
        {
            int permissionId = PermissionsCache.Permissions[permissionCode];

            return GetPermissionIds(user).Any(x => x == permissionId);
        }

        public static string GetName(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value;
        }

        public static LoginTypesEnum GetLoginType(this ClaimsPrincipal user)
        {
            return user.GetIdentityProvider();
        }

        public static string GetClientId(this ClaimsPrincipal user)
        {
            return user.Claims.Where(x => x.Type == JwtClaimTypes.ClientId).Select(x => x.Value).FirstOrDefault();
        }

        public static LoginTypesEnum GetIdentityProvider(this ClaimsPrincipal user)
        {
            return System.Enum.Parse<LoginTypesEnum>(user.Claims.FirstOrDefault(c => c.Type == CustomClaims.LoginType)?.Value);
        }

        public static string GetName(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value;
        }

        public static int? GetUserId(this IPrincipal user)
        {
            if (user != null && user is ClaimsPrincipal)
            {
                return int.Parse(((ClaimsPrincipal)user).Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value);
            }
            else
            {
                return default;
            }
        }

        public static int? GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value);
        }
    }
}
