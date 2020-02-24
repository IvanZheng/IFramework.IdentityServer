using System.Threading.Tasks;
using IdentityServer.Core.Data;
using IdentityServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Core.Managers
{
    public class PermissionManager
    {
        private readonly ApplicationDbContext _dbContext;

        public PermissionManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> IsGrantedAsync(string name,
                                         string providerType,
                                         string providerKey,
                                         string scopeId,
                                         string tenantId)
        {
            if (string.IsNullOrWhiteSpace(scopeId))
            {
                scopeId = null;
            }
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                tenantId = null;
            }

            return _dbContext.PermissionGrants.AnyAsync(pg => pg.Name == name &&
                                                              pg.ProviderType == providerType &&
                                                              pg.ProviderKey == providerKey &&
                                                              pg.ScopeId == scopeId &&
                                                              pg.TenantId == tenantId);
        }

        public async Task<IdentityResult> GrantAsync(string name,
                                                     string providerType,
                                                     string providerKey,
                                                     string scopeId,
                                                     string tenantId)
        {
            if (string.IsNullOrWhiteSpace(scopeId))
            {
                scopeId = null;
            }
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                tenantId = null;
            }

            if (!await _dbContext.PermissionGrants
                                 .AnyAsync(pg => pg.Name == name &&
                                                 pg.ProviderType == providerType &&
                                                 pg.ProviderKey == providerKey &&
                                                 pg.TenantId == tenantId)
                                 .ConfigureAwait(false))
            {
                _dbContext.PermissionGrants.Add(new ApplicationPermissionGrant(name, providerKey, providerType, scopeId, tenantId));
                await _dbContext.SaveChangesAsync();
            }

            return IdentityResult.Success;
        }
    }
}