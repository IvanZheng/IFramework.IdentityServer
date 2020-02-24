using System.Threading.Tasks;
using IdentityServer.Core;
using IdentityServer.Core.Managers;
using IdentityServer4;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizationConsts.AdministrationPolicy)]
    [Route("permissions")]
    public class PermissionController: Controller
    {
        private readonly PermissionManager _permissionManager;

        public PermissionController(PermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }

        [HttpGet]
        public Task<bool> IsGrantedAsync(string name,
                                         string providerType,
                                         string providerKey,
                                         string scopeId,
                                         string tenantId)
        {
            return _permissionManager.IsGrantedAsync(name,
                                                     providerType,
                                                     providerKey,
                                                     scopeId,
                                                     tenantId);
        }
    }
}
