using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Api.DTO.Requests;
using IdentityServer.Core;
using IdentityServer.Core.Models;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers
{
    [ApiController]
    [Authorize(AuthorizationConsts.AdministrationPolicy)]
    [Route("users")]
    public class UserController:Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IdentityResult> Register(Register registerModel)
        {
            var user = new ApplicationUser(registerModel.UserName);
            var result = await _userManager.CreateAsync(user, registerModel.Password)
                                           .ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            user = await _userManager.FindByNameAsync(user.UserName)
                                     .ConfigureAwait(false);
            result = await _userManager.AddClaimsAsync(user,
                                            new[]
                                            {
                                                new Claim(JwtClaimTypes.Name, registerModel.Name), 
                                                new Claim(JwtClaimTypes.GivenName, registerModel.GivenName), 
                                                new Claim(JwtClaimTypes.FamilyName, registerModel.FamilyName), 
                                                new Claim(JwtClaimTypes.Email, registerModel.Email),
                                                new Claim(JwtClaimTypes.EmailVerified, registerModel.EmailConfirmed, ClaimValueTypes.Boolean)
                                                //new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                                            });
            return result;
        }
    }
}
