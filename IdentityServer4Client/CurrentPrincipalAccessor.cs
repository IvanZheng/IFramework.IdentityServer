using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4Client
{
    public class CurrentPrincipalAccessor: ICurrentPrincipalAccessor
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrentPrincipalAccessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ClaimsPrincipal GetCurrentPrincipal()
        {
            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();

            return httpContextAccessor?.HttpContext?.User ?? ClaimsPrincipal.Current;
        }

        public ClaimsPrincipal Principal => GetCurrentPrincipal();
    }
}
