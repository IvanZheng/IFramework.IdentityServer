using System.Security.Claims;
using System.Threading.Tasks;
using Api.Authorizations;
using Microsoft.AspNetCore.Authorization;

namespace IdentityServer4Client.Authorizations
{
    public static class AuthorizationServiceExtensions
    {
        public static Task<AuthorizationResult> AuthorizePermissionAsync(this IAuthorizationService service,
                                                               ClaimsPrincipal user,
                                                               object resource,
                                                               string permission)
        {
            return service.AuthorizeAsync(user,
                                          resource,
                                          new PermissionScopeRequirement(permission));
        }
    }
}