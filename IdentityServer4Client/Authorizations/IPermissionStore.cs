using System.Threading.Tasks;
using Refit;

namespace IdentityServer4Client.Authorizations
{
    public interface IPermissionStore
    {
        [Get("/permissions")]
        Task<bool> IsGrantedAsync(string name,
                                  string providerType,
                                  string providerKey,
                                  string scopeId,
                                  string tenantId);
    }
}