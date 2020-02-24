using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Managers
{
    public class RoleValidator : RoleValidator<ApplicationRole>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="T:Microsoft.AspNetCore.Identity.RoleValidator`1" />/
        /// </summary>
        /// <param name="errors">
        ///     The <see cref="T:Microsoft.AspNetCore.Identity.IdentityErrorDescriber" /> used to provider error
        ///     messages.
        /// </param>
        public RoleValidator(IdentityErrorDescriber errors = null)
        {
            Describer = errors ?? new IdentityErrorDescriber();
        }

        private IdentityErrorDescriber Describer { get; }

        public override async Task<IdentityResult> ValidateAsync(RoleManager<ApplicationRole> manager, ApplicationRole role)
        {
            if (string.IsNullOrWhiteSpace(role.ScopeId))
            {
                return await base.ValidateAsync(manager, role);
            }
            else
            {
                var roleManager = manager as RoleManager;
                if (manager == null)
                    throw new ArgumentNullException(nameof (manager));
                List<IdentityError> errors = new List<IdentityError>();
                await ValidateRoleName(roleManager, role,  errors);
                return errors.Count <= 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
            }
        }

  

        private async Task ValidateRoleName(RoleManager manager,
                                            ApplicationRole role,
                                            ICollection<IdentityError> errors)
        {
            string roleName = await manager.GetRoleNameAsync(role);
            if (string.IsNullOrWhiteSpace(roleName))
            {
                errors.Add(Describer.InvalidRoleName(roleName));
            }
            else
            {
                var byNameScopeAsync = await manager.FindByNameScopeAsync(roleName, role.ScopeId);
                bool flag = byNameScopeAsync != null;
                if (flag)
                {
                    string a = await manager.GetRoleIdAsync(byNameScopeAsync);
                    flag = !string.Equals(a, await manager.GetRoleIdAsync(role));
                    a = null;
                }

                if (!flag)
                {
                    return;
                }

                errors.Add(Describer.DuplicateRoleName(roleName));
            }
        }
    }
}