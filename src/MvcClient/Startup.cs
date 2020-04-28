using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Logging;
using MvcClient.Model;
using Newtonsoft.Json;
using OAuth2Client;

namespace MvcClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddMvc(options => options.EnableEndpointRouting = false);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


            services.AddClientCredentialsClient<IApi>("http://localhost:5001",
                                                      "http://localhost:5000/connect/token",
                                                      "mvc",
                                                      "secret",
                                                      "api1");

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies",options =>
                {
                    options.Events = new CookieAuthenticationEvents
                    {
                        // this event is fired everytime the cookie has been validated by the cookie middleware,
                        // so basically during every authenticated request
                        // the decryption of the cookie has already happened so we have access to the user claims
                        // and cookie properties - expiration, etc..
                        OnValidatePrincipal = x => OAuth2ClientConst.OnValidatePrincipal("http://localhost:5000/connect/token",
                                                                                         "mvc",
                                                                                         "secret",
                                                                                         x)
                    };
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Events.OnUserInformationReceived = context =>
                    {
                        return Task.CompletedTask;
                    };


                    options.SignInScheme = "Cookies";

                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "mvc";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("api1");
                    options.Scope.Add("api2");
                    options.Scope.Add("offline_access");
                    options.Scope.Add(JwtClaimTypes.Role);
                    //options.ClaimActions.MapAll();
                    options.ClaimActions.MapAllExcept("role", "iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash", "auth_time", "idp", "amr");
                    options.ClaimActions.Add(new JsonKeyClaimAction(JwtClaimTypes.Role, null, JwtClaimTypes.Role));
                  
                    //options.ClaimActions.MapJsonKey("website", "website");
                    //options.ClaimActions.MapJsonKey(JwtClaimTypes.Role, JwtClaimTypes.Role, JwtClaimTypes.Role);
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}