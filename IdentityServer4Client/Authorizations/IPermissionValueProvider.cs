using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4Client.Authorizations;

namespace Api.Authorizations
{
    public interface IPermissionValueProvider
    {
        string Name { get; }

        Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context);
    }
}
