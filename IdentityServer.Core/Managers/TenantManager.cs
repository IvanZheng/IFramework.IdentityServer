using System;
using System.Threading.Tasks;
using IdentityServer.Core.Data;
using IdentityServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Core.Managers
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

        public async Task<IdentityResult> CreateAsync(string name, string nodeId = null)
        {
            var tenant = new ApplicationTenant(name); 
            _dbContext.Tenants.Add(tenant);
            
            var tenantNode = new ApplicationNode(name, tenant.Id, nodeId:nodeId);
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
