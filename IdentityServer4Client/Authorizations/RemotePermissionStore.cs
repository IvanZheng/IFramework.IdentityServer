using System.Threading.Tasks;

namespace IdentityServer4Client.Authorizations
{
    public class RemotePermissionStore:IPermissionStore
    {
        public Task<bool> IsGrantedAsync(string name, string providerType, string providerKey, string scopeId, string tenantId)
        {
             
            return Task.FromResult(true);
        }
    }
}
