using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Security;
using IdentityServer4Client.Authorizations;

namespace Api.Authorizations
{
    public class PermissionDefinitionProvider: IPermissionDefinitionProvider
    {
        public void Define(Dictionary<string, PermissionGroupDefinition> groups)
        {
            var permissionGroupDefinition = new PermissionGroupDefinition(nameof(Api), ApiManagementPermissions.GroupName);
            permissionGroupDefinition.AddPermission(ApiManagementPermissions.Get);
            permissionGroupDefinition.AddPermission(ApiManagementPermissions.Post);
            groups.Add(permissionGroupDefinition.Name, permissionGroupDefinition);
        }
    }
}
