using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Authorizations;
using Api.Security;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4Client.Authorizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("identity")]
    public class IdentityController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IServiceProvider _serviceProvider;

        public IdentityController(IServiceProvider serviceProvider, IAuthorizationService authorizationService)
        {
            _serviceProvider = serviceProvider;
            _authorizationService = authorizationService;
        }

        [Authorize(Policy = "policy1")]
        [HttpGet]
        public IActionResult Get(string scopeId)
        {
            return new JsonResult(from c in User.Claims select new {c.Type, c.Value});
        }

        [Authorize("policy2")]
        //[Authorize(ApiManagementPermissions.Post)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] dynamic data)
        {
            var result = await _authorizationService.AuthorizePermissionAsync(User,
                                                                              (string)data.ScopeId,
                                                                              ApiManagementPermissions.Post);
            if (result.Succeeded)
            {
                return new JsonResult(data);
            }
            else if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();

            }
        }
    }
}