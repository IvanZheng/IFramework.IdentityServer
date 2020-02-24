using System.Collections.Generic;

namespace IdentityServer.Core
{
    public class AuthenticationConsts
    {
        public const string IdentityAdminCookieName = "IdentityServerAdmin";
        public const string UserNameClaimType = "name";
        public const string SignInScheme = "Cookies";
        public const string OidcClientId = "zero_identity_admin";
        public const string OidcClientSecret = "zero_admin_client_secret";
        public const string OidcAuthenticationScheme = "oidc";
        public const string OidcResponseType = "code id_token";
        public static List<string> Scopes = new List<string> { ScopeOpenId, ScopeProfile, ScopeEmail, ScopeRoles };

        public const string IdentityApiSwaggerClientId = "zero_identity_api_swaggerui";
        public const string IdentityApiScope = "zero_identity_api";

        public const string ScopeOpenId = "openid";
        public const string ScopeProfile = "profile";
        public const string ScopeEmail = "email";
        public const string ScopeRoles = "roles";

        public const string RoleClaim = "role";

        public const string AccountLoginPage = "Account/Login";
        public const string AccountAccessDeniedPage = "/Account/AccessDenied/";
    }
    public class ApiConfigurationConsts
    {
        public const string ApiName = "Zero IdentityServer4 Api";

        public const string ApiVersionV1 = "v1";
    }
    public class AuthorizationConsts
    {
        public const string IdentityServerBaseUrl = "http://localhost:5000";
        public const string OidcSwaggerUIClientId = "zero_identity_api_swaggerui";
        public const string OidcApiName = "zero_identity_api";

        public const string AdministrationPolicy = "RequireAdministratorRole";
        public const string AdministrationRole = "zero.Admin";
    }

    public class AdminApiConfiguration
    {
        public string IdentityServerBaseUrl { get; set; } = AuthorizationConsts.IdentityServerBaseUrl;

        public string OidcSwaggerUIClientId { get; set; } = AuthorizationConsts.OidcSwaggerUIClientId;

        public string OidcApiName { get; set; } = AuthorizationConsts.OidcApiName;
    }
}
