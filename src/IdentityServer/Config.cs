using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using IdentityModel;
using IdentityServer.Core;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServerAspNetIdentity
{
    public static class Config
    {
        /// <summary>
    /// Impl of adding a signin key for identity server 4,
    /// with an appsetting.json configuration look similar to:
    /// "SigninKeyCredentials": {
    ///     "KeyType": "KeyFile",
    ///     "KeyFilePath": "C:\\certificates\\idsv4.pfx",
    ///     "KeyStorePath": ""
    /// }
    /// </summary>
        private const string KeyType = "KeyType";
        private const string KeyTypeKeyFile = "KeyFile";
        private const string KeyTypeKeyStore = "KeyStore";
        private const string KeyTypeTemporary = "Temporary";
        private const string KeyFilePath = "KeyFilePath";
        private const string KeyFilePassword = "KeyFilePassword";
        private const string KeyStoreIssuer = "KeyStoreIssuer";
        private const string IdentityApiScope = "zero_identity_api";
        public static IIdentityServerBuilder AddSigninCredentialFromConfig(
            this IIdentityServerBuilder builder, IConfigurationSection options, ILogger logger)
        {
            string keyType = options.GetValue<string>(KeyType);
            logger.LogDebug($"SigninCredentialExtension keyType is {keyType}");

            switch (keyType)
            {
                case KeyTypeTemporary:
                    logger.LogDebug($"SigninCredentialExtension adding Temporary Signing Credential");
                    builder.AddDeveloperSigningCredential();
                    break;

                case KeyTypeKeyFile:
                    AddCertificateFromFile(builder, options, logger);
                    break;

                case KeyTypeKeyStore:
                    AddCertificateFromStore(builder, options, logger);
                    break;
            }

            return builder;
        }

        private static void AddCertificateFromStore(IIdentityServerBuilder builder, 
            IConfigurationSection options, ILogger logger)
        {
            var keyIssuer = options.GetValue<string>(KeyStoreIssuer);
            logger.LogDebug($"SigninCredentialExtension adding key from store by {keyIssuer}");

            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var certificates = store.Certificates.Find(X509FindType.FindByIssuerName, keyIssuer, true);

            if (certificates.Count > 0)
                builder.AddSigningCredential(certificates[0]);
            else
                logger.LogError("A matching key couldn't be found in the store");
        }

        private static void AddCertificateFromFile(IIdentityServerBuilder builder, 
            IConfigurationSection options, ILogger logger)
        {
            var keyFilePath = options.GetValue<string>(KeyFilePath);
            var keyFilePassword = options.GetValue<string>(KeyFilePassword);

            if (File.Exists(keyFilePath))
            {
                logger.LogDebug($"SigninCredentialExtension adding key from file {keyFilePath}");
                builder.AddSigningCredential(new X509Certificate2(keyFilePath, keyFilePassword));
            }
            else
            {
                logger.LogError($"SigninCredentialExtension cannot find key file {keyFilePath}");
            }
        }
    

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource(JwtClaimTypes.Role, JwtClaimTypes.Role, new []{JwtClaimTypes.Role})
            };
        }

        public static IEnumerable<ApiResource> GetApis(AdminConfiguration adminConfiguration)
        {
            return new List<ApiResource>
            {
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName),
                new ApiResource(adminConfiguration.IdentityAdminApiScope, new List<string>{JwtClaimTypes.Role})
                {
                    Scopes = new List<Scope>()
                    {
                        new Scope(adminConfiguration.IdentityAdminApiScope)
                    }
                },
                new ApiResource("api1", "My API", new List<string>{JwtClaimTypes.Role})
                {
                    ApiSecrets = new List<Secret> 
                    {
                        new Secret("api1Secret1".Sha256(), "api1Secret1", DateTime.Now.AddMinutes(1)),
                        new Secret("api1Secret2".Sha256(), "api1Secret2")

                    },
                    Scopes = new List<Scope>
                    {
                        new Scope("api1"),
                        new Scope("api2"), 
                        new Scope("apiall")
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients(AdminConfiguration adminConfiguration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256(), "secret1", DateTime.Now.AddMinutes(1)), 
                        new Secret("secret2".Sha256(), "secret2")
                    },

                    // scopes that client has access to
                    AllowedScopes = {
                        "api1", 
                        "api2",
                        "apiall",
                        JwtClaimTypes.Role,
                        IdentityServerConstants.LocalApi.ScopeName,
                        adminConfiguration.IdentityAdminApiScope
                    },
                    ClientClaimsPrefix = string.Empty,
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Role, "zero.Admin")
                    }
                },
                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client", 
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = {new Secret("secret".Sha256())},
                    AllowedScopes = {"api1", adminConfiguration.IdentityAdminApiScope}
                },
                // OpenID Connect hybrid flow client (MVC)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets = {new Secret("secret".Sha256())},
                    RedirectUris = {"http://localhost:5002/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5002/signout-callback-oidc"},
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile,
                        JwtClaimTypes.Role,
                        "api1",
                        "api2",
                        adminConfiguration.IdentityAdminApiScope
                    },
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "mvc2",
                    ClientName = "MVC Client2",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets = {new Secret("secret".Sha256())},
                    RedirectUris = {"http://localhost:5003/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5003/signout-callback-oidc"},
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile,
                        JwtClaimTypes.Role,
                        "api1",
                        "api2",
                        adminConfiguration.IdentityAdminApiScope
                    },
                    AllowOfflineAccess = true
                },
                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RedirectUris = {"http://localhost:5003/callback.html"},
                    PostLogoutRedirectUris = {"http://localhost:5003/index.html"},
                    AllowedCorsOrigins = {"http://localhost:5003"},
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile, 
                        "api1",
                        adminConfiguration.IdentityAdminApiScope
                    }
                },
                new Client
                {
                    ClientId = adminConfiguration.IdentityAdminApiSwaggerUIClientId,
                    ClientName = adminConfiguration.IdentityAdminApiSwaggerUIClientId,
                    
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris = new List<string>
                    {
                        adminConfiguration.IdentityAdminApiSwaggerUIRedirectUrl
                    },
                    AllowedScopes =
                    {
                        JwtClaimTypes.Role,
                        adminConfiguration.IdentityAdminApiScope
                    },
                    AllowAccessTokensViaBrowser = true
                }
            };
        }
    }
}