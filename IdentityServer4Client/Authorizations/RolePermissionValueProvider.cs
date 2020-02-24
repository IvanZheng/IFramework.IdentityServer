using System.Linq;
using System.Threading.Tasks;
using IdentityModel;

namespace IdentityServer4Client.Authorizations
{
    public class RolePermissionValueProvider : PermissionValueProvider
    {
        public const string ProviderName = "Role";

        public override string Name => ProviderName;

        public RolePermissionValueProvider(IPermissionStore permissionStore)
            : base(permissionStore)
        {

        }

        public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
        {
            var roles = context.Principal?.FindAll(JwtClaimTypes.Role)
                               .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                               .Select(c => new Role(c.Value))
                               .ToArray();
            if (!string.IsNullOrWhiteSpace(context.ScopeId))
            {
                roles = roles?.Where(r => r.ScopeId == context.ScopeId)
                              .ToArray();
            }

            if (roles == null || !roles.Any())
            {
                return PermissionGrantResult.Undefined;
            }

            foreach (var role in roles)
            {
                if (await PermissionStore.IsGrantedAsync(context.Permission.Name,
                                                         Name,
                                                         role.Name,
                                                         role.ScopeId,
                                                         context.TenantId))
                {
                    return PermissionGrantResult.Granted;
                }
            }

            return PermissionGrantResult.Undefined;
        }
    }
}
