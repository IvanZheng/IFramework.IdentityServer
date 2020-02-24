using System.Threading.Tasks;
using Api.Authorizations;
using Microsoft.AspNetCore.Authorization;

namespace IdentityServer4Client.Authorizations
{
    public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionChecker _permissionChecker;

        public PermissionRequirementHandler(IPermissionChecker permissionChecker)
        {
            _permissionChecker = permissionChecker;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (await _permissionChecker.IsGrantedAsync(context.User, requirement.PermissionName))
            {
                context.Succeed(requirement);
            }
        }
    }


    public class PermissionScopeRequirementHandler : AuthorizationHandler<PermissionScopeRequirement, string>
    {
        private readonly IPermissionChecker _permissionChecker;

        public PermissionScopeRequirementHandler(IPermissionChecker permissionChecker)
        {
            _permissionChecker = permissionChecker;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, 
                                                             PermissionScopeRequirement requirement, 
                                                             string scope)
        {
            if (await _permissionChecker.IsGrantedAsync(context.User, requirement.PermissionName, scope))
            {
                context.Succeed(requirement);
            }
        }
    }
}
