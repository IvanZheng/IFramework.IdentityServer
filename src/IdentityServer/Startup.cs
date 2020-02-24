using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using IdentityServer.Core;
using IdentityServer.Core.Data;
using IdentityServer.Core.Managers;
using IdentityServer.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;

namespace IdentityServerAspNetIdentity
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var adminConfiguration = Configuration.GetSection(nameof(AdminConfiguration)).Get<AdminConfiguration>();
            services.AddSingleton(adminConfiguration);
            IdentityModelEventSource.ShowPII = true;
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContextPool<ApplicationDbContext>(options =>
                                                            options.UseSqlServer(connectionString));

            services.AddScoped<TenantManager>()
                    .AddScoped<PermissionManager>();
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddRoleManager<RoleManager>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory>();
            services.Replace(new ServiceDescriptor(typeof(IRoleValidator<ApplicationRole>), typeof(RoleValidator), ServiceLifetime.Scoped));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddLocalApiAuthentication();
            //services.Configure<IISOptions>(iis =>
            //{
            //    iis.AuthenticationDisplayName = "Windows";
            //    iis.AutomaticAuthentication = false;
            //});

            var builder = services.AddIdentityServer(options =>
                                  {
                                      options.Events.RaiseErrorEvents = true;
                                      options.Events.RaiseInformationEvents = true;
                                      options.Events.RaiseFailureEvents = true;
                                      options.Events.RaiseSuccessEvents = true;
                                  })
                                  // this adds the config data from DB (clients, resources)
                                  .AddConfigurationStore(options =>
                                  {
                                      options.ConfigureDbContext = b =>
                                          b.UseSqlServer(connectionString,
                                                         sql => sql.MigrationsAssembly(migrationsAssembly));
                                  })
                                  // this adds the operational data from DB (codes, tokens, consents)
                                  .AddOperationalStore(options =>
                                  {
                                      options.ConfigureDbContext = b =>
                                          b.UseSqlServer(connectionString,
                                                         sql => sql.MigrationsAssembly(migrationsAssembly));

                                      // this enables automatic token cleanup. this is optional.
                                      options.EnableTokenCleanup = true;
                                  })
                                  .AddAspNetIdentity<ApplicationUser>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                var cert = new X509Certificate2(Path.Combine(Environment.ContentRootPath, "teamcore.pfx"), "Password01!");
                builder.AddSigningCredential(cert);
                //throw new Exception("need to configure key material");
            }

            services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        // register your IdentityServer with Google at https://console.developers.google.com
                        // enable the Google+ API
                        // set the redirect URI to http://localhost:5000/signin-google
                        options.ClientId = "copy client ID from Google here";
                        options.ClientSecret = "copy client secret from Google here";
                    });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}