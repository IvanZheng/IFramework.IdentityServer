using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace OAuth2Client
{
    public abstract class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1,1);
        protected readonly HttpClient InternalHttpClient;
        protected abstract TokenRequest TokenRequest { get; }
        protected TokenResponse TokenResponse { get; set; }
        protected DateTime TokenExpiredDateTime { get; set; } 
        protected virtual Func<HttpRequestMessage, Task<string>> GetToken => async request =>
        {
            //Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released 
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (TokenResponse == null ||
                    TokenExpiredDateTime <= DateTime.Now ||
                    string.IsNullOrWhiteSpace(TokenResponse.RefreshToken) && (TokenExpiredDateTime - DateTime.Now).TotalMinutes < 1)
                {
                    if (TokenRequest is ClientCredentialsTokenRequest clientCredentialsTokenRequest)
                    {
                        TokenResponse = await InternalHttpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);
                    }
                    else if (TokenRequest is PasswordTokenRequest passwordTokenRequest)
                    {
                        TokenResponse = await InternalHttpClient.RequestPasswordTokenAsync(passwordTokenRequest);
                    }
                    //else if (TokenRequest is AuthorizationCodeTokenRequest authorizationCodeTokenRequest)
                    //{
                    //    TokenResponse = await InternalHttpClient.RequestAuthorizationCodeTokenAsync(authorizationCodeTokenRequest);
                    //}
                    //else if (TokenRequest is DeviceTokenRequest deviceTokenRequest)
                    //{
                    //    TokenResponse = await InternalHttpClient.RequestDeviceTokenAsync(deviceTokenRequest);
                    //}
                    else
                    {
                        throw new Exception($"Invalid TokenRequest {Newtonsoft.Json.JsonConvert.SerializeObject(TokenRequest)}");
                    }
                }
                else if (!string.IsNullOrWhiteSpace(TokenResponse.RefreshToken) && (TokenExpiredDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    // Refresh Token
                    TokenResponse = await InternalHttpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
                    {
                        Address = TokenRequest.Address,
                        RefreshToken = TokenResponse.RefreshToken,
                        ClientId = TokenRequest.ClientId,
                        ClientSecret = TokenRequest.ClientSecret
                    });
                }

                if (TokenResponse.IsError)
                {
                    throw new Exception(TokenResponse.Error);
                }

                TokenExpiredDateTime = DateTime.Now.AddSeconds(TokenResponse.ExpiresIn);
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }
            return TokenResponse.AccessToken;
        };

        protected AuthenticatedHttpClientHandler(IHttpClientFactory httpClientFactory)
        {
            InternalHttpClient = httpClientFactory.CreateClient(nameof(AuthenticatedHttpClientHandler));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // See if the request has an authorize header
            var auth = request.Headers.Authorization;
            if (auth != null && string.IsNullOrWhiteSpace(auth.Parameter))
            {
                var token = await GetToken(request).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
                }
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
