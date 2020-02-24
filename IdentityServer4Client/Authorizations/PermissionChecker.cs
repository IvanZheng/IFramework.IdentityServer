using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Authorizations;

namespace IdentityServer4Client.Authorizations
{
    public class PermissionChecker : IPermissionChecker
    {
        protected IPermissionDefinitionManager PermissionDefinitionManager { get; }
        protected ICurrentPrincipalAccessor PrincipalAccessor { get; }
        protected IPermissionValueProviderManager PermissionValueProviderManager { get; }

        public PermissionChecker(
            ICurrentPrincipalAccessor principalAccessor,
            IPermissionDefinitionManager permissionDefinitionManager, 
            IPermissionValueProviderManager permissionValueProviderManager)
        {
            PrincipalAccessor = principalAccessor;
            PermissionDefinitionManager = permissionDefinitionManager;
            PermissionValueProviderManager = permissionValueProviderManager;
        }

        public virtual Task<bool> IsGrantedAsync(string name)
        {
            return IsGrantedAsync(PrincipalAccessor.Principal, name);
        }
     
        public async Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, string name, string scope = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            var permission = PermissionDefinitionManager.Get(name);
            var isGranted = false;
            var context = new PermissionValueCheckContext(permission, claimsPrincipal, scope);
            foreach (var provider in PermissionValueProviderManager.ValueProviders)
            {
                var result = await provider.CheckAsync(context);

                if (result == PermissionGrantResult.Granted)
                {
                    isGranted = true;
                }
                else if (result == PermissionGrantResult.Prohibited)
                {
                    return false;
                }
            }

            return isGranted;
        }
    }
}
