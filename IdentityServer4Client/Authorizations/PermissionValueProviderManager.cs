using System;
using System.Collections.Generic;
using System.Linq;
using Api.Authorizations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityServer4Client.Authorizations
{
    public class PermissionValueProviderManager : IPermissionValueProviderManager
    {
        private readonly Lazy<List<IPermissionValueProvider>> _lazyProviders;

        public PermissionValueProviderManager(IServiceProvider serviceProvider,
                                              IOptions<PermissionOptions> options)
        {
            Options = options.Value;

            _lazyProviders = new Lazy<List<IPermissionValueProvider>>(() =>
                                                                      {
                                                                          return Options
                                                                                 .ValueProviders
                                                                                 .Select(c => serviceProvider.GetRequiredService(c) as IPermissionValueProvider)
                                                                                 .ToList();
                                                                      },
                                                                      true);
        }

        protected PermissionOptions Options { get; }
        public IReadOnlyList<IPermissionValueProvider> ValueProviders => _lazyProviders.Value;
    }
}