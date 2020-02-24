using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServerAspNetIdentity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Managers
{
    public class TenantManager:IDisposable 
    {
        private readonly ApplicationDbContext _dbContext;

        public TenantManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
        }

        public async Task<IdentityResult> CreateAsync(string name)
        {
            var tenant = new ApplicationTenant(name); 
            _dbContext.Tenants.Add(tenant);
            
            var tenantNode = new ApplicationNode(name, tenant.Id);
            _dbContext.Nodes.Add(tenantNode);

            tenant.NodeId = tenantNode.Id;

            await _dbContext.SaveChangesAsync()
                            .ConfigureAwait(false);
            return IdentityResult.Success;
        }

        public async Task<ApplicationTenant> FindByNameAsync(string name)
        {
            var tenant = await _dbContext.Tenants
                                         .FirstOrDefaultAsync(t => t.Name == name)
                                         .ConfigureAwait(false);
            return tenant;
        }
    }
}
