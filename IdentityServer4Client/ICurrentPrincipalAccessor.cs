using System.Security.Claims;

namespace IdentityServer4Client
{
    public interface ICurrentPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }
    }
}
