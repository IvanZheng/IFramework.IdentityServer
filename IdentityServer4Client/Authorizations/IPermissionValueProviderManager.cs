using System.Collections.Generic;
using Api.Authorizations;

namespace IdentityServer4Client.Authorizations
{
    public interface IPermissionValueProviderManager
    {
        IReadOnlyList<IPermissionValueProvider> ValueProviders { get; }
    }
}
