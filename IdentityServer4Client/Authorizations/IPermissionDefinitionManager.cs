using System.Collections.Generic;
using Api.Authorizations;

namespace IdentityServer4Client.Authorizations
{
    public interface IPermissionDefinitionManager
    {
        PermissionDefinition Get(string name);

        PermissionDefinition GetOrNull(string name);

        IReadOnlyList<PermissionDefinition> GetPermissions();

        IReadOnlyList<PermissionGroupDefinition> GetGroups();
    }
}
