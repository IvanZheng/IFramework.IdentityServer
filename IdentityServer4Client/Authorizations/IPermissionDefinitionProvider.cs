using System.Collections.Generic;
using Api.Authorizations;

namespace IdentityServer4Client.Authorizations
{
    public interface IPermissionDefinitionProvider
    {
        void Define(Dictionary<string, PermissionGroupDefinition> groups);

    }
}
