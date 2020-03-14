using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Core;
using IdentityServer.Core.Data;
using IdentityServer.Core.Managers;
using IdentityServer.Core.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace IdentityServer.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var adminApiConfiguration = Configuration.GetSection(nameof(AdminApiConfiguration)).Get<AdminApiConfiguration>();
            services.AddSingleton(adminApiConfiguration);
            IdentityModelEventSource.ShowPII = true;
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;
            services.AddMvcCore()
                    .AddAuthorizationPolicies()
                    .AddApiExplorer();
            services.AddDbContextPool<ApplicationDbContext>(options =>
                                                                options.UseSqlServer(connectionString));

            services.AddConfigurationDbContext<ConfigurationDbContext>(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString,
                                   sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            // Operational DB from existing connection
            services.AddOperationalDbContext<PersistedGrantDbContext>(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString,
                                   sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            services.AddApiAuthentication<ApplicationDbContext, ApplicationUser, ApplicationRole>(adminApiConfiguration);
           

            services.AddScoped<TenantManager>()
                    .AddScoped<PermissionManager>();
          
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory>();
            services.Replace(new ServiceDescriptor(typeof(IRoleValidator<ApplicationRole>), typeof(RoleValidator), ServiceLifetime.Scoped));
            

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(ApiConfigurationConsts.ApiVersionV1, 
                                   new Info { 
                                       Title = ApiConfigurationConsts.ApiName,
                                       Version = ApiConfigurationConsts.ApiVersionV1
                                   });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "implicit",
                    AuthorizationUrl = $"{adminApiConfiguration.IdentityServerBaseUrl}/connect/authorize",
                    Scopes = new Dictionary<string, string> {
                        { adminApiConfiguration.OidcApiName, ApiConfigurationConsts.ApiName }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,  AdminApiConfiguration adminApiConfiguration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseCors("default");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", ApiConfigurationConsts.ApiName);

                c.OAuthClientId(adminApiConfiguration.OidcSwaggerUIClientId);
                c.OAuthAppName(ApiConfigurationConsts.ApiName);
            });
            app.UseAuthentication();
          
            app.UseMvc();
        }
    }
}
