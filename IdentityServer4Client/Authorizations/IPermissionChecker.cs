using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4Client.Authorizations
{
    public interface IPermissionChecker
    {
        Task<bool> IsGrantedAsync(string name);

        Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, string name, string scope = null);
    }
}
