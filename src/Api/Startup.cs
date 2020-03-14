// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Api.Authorizations;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4Client;
using IdentityServer4Client.Authorizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                    .AddAuthorization(options =>
                    {
                        options.AddPolicy("policy1", builder =>
                        {
                            // require scope1
                            builder.RequireScope("api1", "apiall");
                        });
                        options.AddPolicy("policy2", builder => { builder.RequireScope("api1", "api2", "apiall"); });
                    });

            
            //services.AddAuthentication("Bearer")
            //        .AddJwtBearer("Bearer", options =>
            //        {
            //            options.Authority = "http://localhost:5000";
            //            options.RequireHttpsMetadata = false;

            //            options.Audience = "api1";
            //        });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "api1";
                    options.ApiSecret = "api1Secret1".ToSha256();
                });
            services.Replace(new ServiceDescriptor(typeof(IAuthorizationPolicyProvider),
                                                   typeof(PermissionAuthorizationPolicyProvider), 
                                                   ServiceLifetime.Singleton));
            services.AddSingleton<IPermissionDefinitionManager, PermissionDefinitionManager>();
            services.AddSingleton<PermissionDefinitionProvider>();
            //services.AddScoped<IPermissionStore, RemotePermissionStore>();
            services.AddIdentityServerApiClient();
            services.AddScoped<IPermissionChecker, PermissionChecker>();
            services.AddScoped<ICurrentPrincipalAccessor, CurrentPrincipalAccessor>();
            services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, PermissionScopeRequirementHandler>();
            services.AddScoped<UserPermissionValueProvider>();
            services.AddScoped<RolePermissionValueProvider>();
            services.AddScoped<IPermissionValueProviderManager, PermissionValueProviderManager>();

            services.Configure<PermissionOptions>(options =>
            {
                options.DefinitionProviders.Add(typeof(PermissionDefinitionProvider));
                options.ValueProviders.Add(typeof(UserPermissionValueProvider));
                options.ValueProviders.Add(typeof(RolePermissionValueProvider));
            });
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("http://localhost:5003")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("default");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}