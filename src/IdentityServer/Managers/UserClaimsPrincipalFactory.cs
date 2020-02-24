using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityServer.Managers
{
    public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly RoleManager _roleManager;

        public UserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
                                          RoleManager roleManager,
                                          IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
            _roleManager = roleManager;
        }


        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            string userId = await UserManager.GetUserIdAsync(user);
            string userNameAsync = await UserManager.GetUserNameAsync(user);
            ClaimsIdentity id = new ClaimsIdentity("Identity.Application",
                                                   Options.ClaimsIdentity.UserNameClaimType,
                                                   Options.ClaimsIdentity.RoleClaimType);
            id.AddClaim(new Claim(Options.ClaimsIdentity.UserIdClaimType, userId));
            id.AddClaim(new Claim(Options.ClaimsIdentity.UserNameClaimType, userNameAsync));
            ClaimsIdentity claimsIdentity;
            if (UserManager.SupportsUserSecurityStamp)
            {
                claimsIdentity = id;
                string type = Options.ClaimsIdentity.SecurityStampClaimType;
                claimsIdentity.AddClaim(new Claim(type, await UserManager.GetSecurityStampAsync(user)));
            }

            if (UserManager.SupportsUserClaim)
            {
                claimsIdentity = id;
                claimsIdentity.AddClaims(await UserManager.GetClaimsAsync(user));
            }

            if (UserManager.SupportsUserRole)
            {
                foreach (var role in await _roleManager.GetRolesByUserAsync(user))
                {
                    var roleClaimValue = role.Name;
                    if (!string.IsNullOrWhiteSpace(role.ScopeId))
                    {
                        roleClaimValue += $":{role.ScopeId}";
                    }
                    var roleClaim = new Claim(Options.ClaimsIdentity.RoleClaimType, roleClaimValue);
                    id.AddClaim(roleClaim);
                }
            }
            return id;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;
            //var claims = new List<Claim>
            //{
            //    new Claim(JwtClaimTypes.Role, "dataEventRecords"),
            //    new Claim(JwtClaimTypes.Role, "dataEventRecords.user")
            //};

            //if (user.DataEventRecordsRole == "dataEventRecords.admin")
            //{
            //    claims.Add(new Claim(JwtClaimTypes.Role, "dataEventRecords.admin"));
            //}

            //if (user.IsAdmin)
            //{
            //    claims.Add(new Claim(JwtClaimTypes.Role, "admin"));
            //}
            //else
            //{
            //    claims.Add(new Claim(JwtClaimTypes.Role, "user"));
            //}

            //identity.AddClaims(claims);
            return principal;
        }
    }
}