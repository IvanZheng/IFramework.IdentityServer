using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Core;
using IdentityServer.Core.Managers;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Api
{
    public static class StartupHelper
    {
        /// <summary>
        /// Add authentication middleware for an API
        /// </summary>
        /// <typeparam name="TIdentityDbContext">DbContext for an access to Identity</typeparam>
        /// <typeparam name="TUser">Entity with User</typeparam>
        /// <typeparam name="TRole">Entity with Role</typeparam>
        /// <param name="services"></param>
        /// <param name="adminApiConfiguration"></param>
        public static void AddApiAuthentication<TIdentityDbContext, TUser, TRole>(this IServiceCollection services,
                                                                                  AdminApiConfiguration adminApiConfiguration) 
            where TIdentityDbContext : DbContext 
            where TRole : class 
            where TUser : class
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = adminApiConfiguration.IdentityServerBaseUrl;
                        options.ApiName = adminApiConfiguration.OidcApiName;

#if DEBUG
                        options.RequireHttpsMetadata = false;
#else
                    options.RequireHttpsMetadata = true;
#endif
                    });

            services.AddIdentity<TUser, TRole>(options =>
                    {
                        options.User.RequireUniqueEmail = true;
                    })
                    .AddRoleManager<RoleManager>()
                    .AddEntityFrameworkStores<TIdentityDbContext>()
                    .AddDefaultTokenProviders();
        }

        public static IMvcCoreBuilder AddAuthorizationPolicies(this IMvcCoreBuilder services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
                                  policy =>
                                  {
                                      policy.RequireScope(AuthenticationConsts.IdentityApiScope);
                                      policy.RequireRole(AuthorizationConsts.AdministrationRole);
                                  });
            });
            return services;
        }
    }
}
