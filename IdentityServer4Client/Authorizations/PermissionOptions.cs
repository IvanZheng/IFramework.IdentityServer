using System;
using System.Collections.Generic;

namespace IdentityServer4Client.Authorizations
{
    public class PermissionOptions
    {
        public IList<Type> DefinitionProviders { get; }

        public IList<Type> ValueProviders { get; }

        public PermissionOptions()
        {
            ValueProviders = new List<Type>();

            DefinitionProviders = new List<Type>();
        }
    }
}
