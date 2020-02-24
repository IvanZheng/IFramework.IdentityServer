using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer.Core;
using IdentityServer.Core.Data;
using IdentityServer.Core.Managers;
using IdentityServer.Core.Models;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerAspNetIdentity
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider provider)
        {
            provider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
            provider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            provider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
            var adminConfiguration = provider.GetRequiredService<AdminConfiguration>();
            {
                var tenantMgr = provider.GetRequiredService<TenantManager>();
                var roleMgr = provider.GetRequiredService<RoleManager>();
                var userMgr = provider.GetRequiredService<UserManager<ApplicationUser>>();
                var permissionManager = provider.GetRequiredService<PermissionManager>();
                //var userRoleStore = provider.GetRequiredService<IUserRoleStore<ApplicationUser>>() as UserRoleStore;

                var tenant1 = tenantMgr.FindByNameAsync("tenant1").Result;
                if (tenant1 == null)
                {
                    var result = tenantMgr.CreateAsync("tenant1", "3d0a1642-c2e1-4031-96a9-4fc651c245c1").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    tenant1 = tenantMgr.FindByNameAsync("tenant1").Result;
                    Console.WriteLine("tenant1 created");
                }
                else
                {
                    Console.WriteLine("tenant1 already exists");
                }

                var tenant2 = tenantMgr.FindByNameAsync("tenant2").Result;
                if (tenant2 == null)
                {
                    var result = tenantMgr.CreateAsync("tenant2").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    tenant2 = tenantMgr.FindByNameAsync("tenant2").Result;
                    Console.WriteLine("tenant2 created");
                }
                else
                {
                    Console.WriteLine("tenant2 already exists");
                }

                var adminRoleName = "zero.Admin";
                var adminRole = roleMgr.FindByNameAsync(adminRoleName).Result;
                if (adminRole == null)
                {
                    var result = roleMgr.CreateAsync(new ApplicationRole(adminRoleName)).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    adminRole = roleMgr.FindByNameAsync(adminRoleName).Result;
                }

                var tenantRoleName = "zero.tenantRole1";
                var tenantAdminRole1 = roleMgr.FindByNameScopeAsync(tenantRoleName, tenant1.NodeId).Result;
                if (tenantAdminRole1 == null)
                {
                    var result = roleMgr.CreateAsync(new ApplicationRole(tenantRoleName, tenant1.NodeId, tenant1.Id)).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    tenantAdminRole1 = roleMgr.FindByNameScopeAsync(tenantRoleName, tenant1.NodeId).Result;
                    Console.WriteLine("tenantAdminRole created");
                }

                var tenantAdminRole2 = roleMgr.FindByNameScopeAsync(tenantRoleName, tenant2.NodeId).Result;
                if (tenantAdminRole2 == null)
                {
                    var result = roleMgr.CreateAsync(new ApplicationRole(tenantRoleName, tenant2.NodeId, tenant2.Id)).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    tenantAdminRole2 = roleMgr.FindByNameScopeAsync(tenantRoleName, tenant2.NodeId).Result;
                    Console.WriteLine("tenantAdminRole created");
                }

                var alice = userMgr.FindByNameAsync("alice").Result;
                if (alice == null)
                {
                    alice = new ApplicationUser {UserName = "alice"};
                    var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    alice = userMgr.FindByNameAsync("alice").Result;
                    result = userMgr.AddClaimsAsync(alice,
                                                    new[]
                                                    {
                                                        new Claim(JwtClaimTypes.Name, "Alice Smith"), 
                                                        new Claim(JwtClaimTypes.GivenName, "Alice"), 
                                                        new Claim(JwtClaimTypes.FamilyName, "Smith"), 
                                                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                                                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                                                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"), 
                                                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                                                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    Console.WriteLine("alice created");


                    result = roleMgr.AddToRoleAsync(alice, tenantAdminRole1).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = roleMgr.AddToRoleAsync(alice, adminRole).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    Console.WriteLine("user role created");
                }
                else
                {
                    Console.WriteLine("alice already exists");
                }

                permissionManager.GrantAsync("Api.ApiManagementPermissions.Post",
                                             "Role",
                                             $"{tenantAdminRole1.Name}",
                                             tenant1.NodeId,
                                             null)
                                 .Wait();


                var bob = userMgr.FindByNameAsync("bob").Result;
                if (bob == null)
                {
                    bob = new ApplicationUser {UserName = "bob"};
                    var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    bob = userMgr.FindByNameAsync("bob").Result;
                    result = userMgr.AddClaimsAsync(bob, new[] {new Claim(JwtClaimTypes.Name, "Bob Smith"), new Claim(JwtClaimTypes.GivenName, "Bob"), new Claim(JwtClaimTypes.FamilyName, "Smith"), new Claim(JwtClaimTypes.Email, "BobSmith@email.com")}).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    Console.WriteLine("bob created");
                    result = roleMgr.AddToRoleAsync(bob, tenantAdminRole2).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                }
                else
                {
                    Console.WriteLine("bob already exists");
                }
            }

            {
                var context = provider.GetRequiredService<ConfigurationDbContext>();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients(adminConfiguration))
                    {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApis(adminConfiguration))
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}