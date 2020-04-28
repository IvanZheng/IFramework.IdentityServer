using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace OAuth2Client
{
    public static class OAuth2ClientConst
    {
        public const string NoneBearerToken = "Bearer";

        public static async Task OnValidatePrincipal(string address, string clientId, string clientSecret, CookieValidatePrincipalContext x)
        {
            var items = x.Properties.Items;
            var tokenExpireAt = DateTimeOffset.MaxValue;
            var expiresAt = items[".Token.expires_at"];
            if (!string.IsNullOrWhiteSpace(expiresAt))
            {
                tokenExpireAt = DateTimeOffset.Parse(expiresAt);
            }

            // since our cookie lifetime is based on the access token one,
            // check if we're more than halfway of the cookie lifetime
            var now = DateTimeOffset.UtcNow;
            if (tokenExpireAt < now)
            {
                var refreshToken = items[".Token.refresh_token"];

                // if we have to refresh, grab the refresh token from the claims, and request
                // new access token and refresh token
                var response = await new HttpClient().RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = address,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    RefreshToken = refreshToken
                });

                if (!response.IsError)
                {
                    // everything went right, remove old tokens and add new ones
                    items[".Token.refresh_token"] = response.RefreshToken;
                    items[".Token.access_token"] = response.AccessToken;
                    items[".Token.expires_at"] = now.AddSeconds(response.ExpiresIn).ToString("O");
                    // indicate to the cookie middleware to renew the session cookie
                    // the new lifetime will be the same as the old one, so the alignment
                    // between cookie and access token is preserved
                    x.ShouldRenew = true;
                }
            }
        }
    }
}