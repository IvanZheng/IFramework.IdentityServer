using System;
using System.Security.Claims;
using IdentityModel;

namespace IdentityServer4Client.Authorizations
{
    public class PermissionValueCheckContext
    {
        public PermissionValueCheckContext(PermissionDefinition permission,
                                           string userId,
                                           string scopeId = null,
                                           string tenantId = null)
        {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
            UserId = userId;
            ScopeId = scopeId;
            TenantId = tenantId;
        }

        public PermissionValueCheckContext(PermissionDefinition permission,
                                           ClaimsPrincipal principal,
                                           string scopeId = null)
        {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
            Principal = principal;
            ScopeId = scopeId;
            TenantId = principal?.FindFirst("tenantId")?.Value;
            UserId = principal?.FindFirst(JwtClaimTypes.Subject)?.Value;

        }

        public PermissionDefinition Permission { get; }
        public ClaimsPrincipal Principal { get; }
        public string ScopeId { get;  }
        public string TenantId { get; }
        public string UserId { get; }
    }
}