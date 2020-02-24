using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Managers
{
    public class RoleManager : RoleManager<ApplicationRole>
    {
        private readonly IRoleStore<ApplicationRole> _store;
        protected ApplicationDbContext Context => (Store as RoleStore<ApplicationRole, ApplicationDbContext, string, IdentityUserRole<string>, IdentityRoleClaim<string>>)?.Context;
        public RoleManager(IRoleStore<ApplicationRole> store,
                           IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
                           ILookupNormalizer keyNormalizer,
                           IdentityErrorDescriber errors,
                           ILogger<RoleManager<ApplicationRole>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _store = store;
        }

        public virtual Task<ApplicationRole> FindByNameScopeAsync(string roleName, string scopeId)
        {
            return Context.Set<ApplicationRole>().FirstOrDefaultAsync(r => r.Name == roleName && r.ScopeId == scopeId);
        }

        public virtual async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role, CancellationToken cancellationToken = default)
        { 
            await (Store as IUserRoleStore<ApplicationUser>).AddToRoleAsync(user, role, cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;
            
            //var context = Context ?? throw new ArgumentNullException($"store");
            //var applicationRole = await context.Roles
            //                                   .FirstOrDefaultAsync(r => r.Name == role && r.ScopeId == null, cancellationToken)
            //                                   .ConfigureAwait(false);
            //return await AddToRoleAsync(user, applicationRole, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role, string scopedId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(role));
            }

            if (string.IsNullOrWhiteSpace(scopedId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(scopedId));
            }

            var context = Context ??
                          throw new ArgumentNullException($"store");
            var applicationRole = await context.Roles
                                               .FirstOrDefaultAsync(r => r.ScopeId == scopedId && r.Name == role, cancellationToken)
                                               .ConfigureAwait(false);
            return await AddToRoleAsync(user, applicationRole, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<ApplicationRole[]> GetRolesByUserAsync(ApplicationUser user)
        {
            var context = Context ??
                          throw new ArgumentNullException($"store");
            var query = from userRole in context.UserRoles.Where(ur => ur.UserId == user.Id)
                        join role in context.Roles on userRole.RoleId equals role.Id
                        select role;
            return await query.ToArrayAsync();
        }


        public virtual async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, ApplicationRole role,  CancellationToken cancellationToken = default)
        {
            var context = Context ??
                          throw new ArgumentNullException($"store");

            cancellationToken.ThrowIfCancellationRequested();
            if ( user == null)
                throw new ArgumentNullException(nameof (user));
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            context.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string> {RoleId = role.Id, UserId = user.Id});
            await context.SaveChangesAsync(cancellationToken)
                         .ConfigureAwait(false);
            return IdentityResult.Success;
        }
    }
}