using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IdentityServer4Client
{
    internal class PermissionStoreHttpHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IdentityServerAuthenticationOptions _options;
        private readonly ILogger _logger;
        private TokenResponse _tokenResponse;
        private DateTime? _expiresAt;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public PermissionStoreHttpHandler(ILogger<PermissionStoreHttpHandler> logger,
                                          IOptionsMonitor<IdentityServerAuthenticationOptions> optionsMonitor,
                                          IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = optionsMonitor.Get("Bearer");
        }


        private async Task<TResult> LockAsync<TResult>(Func<Task<TResult>> worker)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await worker();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        protected virtual async Task<string> GetAccessTokenAsync()
        {
            if (_tokenResponse == null || _expiresAt < DateTime.Now.AddMinutes(5))
            {
                var client = _httpClientFactory.CreateClient(Consts.IdentityServerAuthenticationHttpClient);
                var disco = await client.GetDiscoveryDocumentAsync(_options.Authority);
                if (disco.IsError)
                {
                    throw new Exception(disco.Error);
                }
                // request token
                _tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint, 
                    ClientId = "client", 
                    ClientSecret = "secret2",
                    Scope = "zero_identity_api"
                });
                if (_tokenResponse.IsError)
                {
                    throw new Exception(_tokenResponse.Error);
                }
                _expiresAt = DateTime.Now.AddSeconds(_tokenResponse.ExpiresIn);
            }

            return _tokenResponse.AccessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var startDate = DateTime.Now;
            HttpResponseMessage response = null;
            try
            {
                request.SetBearerToken(await LockAsync(GetAccessTokenAsync));
                response = await base.SendAsync(request, cancellationToken);
                await LogInfo(startDate, request, response).ConfigureAwait(false);
                return response;
            }
            catch (Exception e)
            {
                await LogInfo(startDate, request, response, e).ConfigureAwait(false);
                throw;
            }
        }

        private async Task LogInfo(DateTime startDate, HttpRequestMessage request, HttpResponseMessage response, Exception exception = null)
        {
            var endDate = DateTime.Now;
            var costTime = (endDate - startDate).TotalMilliseconds;

            var responseBody = request.Method != HttpMethod.Get && response?.Content != null
                                   ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                                   : null;
            //requestBody= HttpUtility.UrlDecode(requestBody);
            var requestJson = JsonConvert.SerializeObject(new
            {
                request.RequestUri,
                request.Method,
                responseBody,
                response?.StatusCode,
                costTime
            });

            if (exception != null)
            {
                _logger.LogError(exception, requestJson);
            }
            else if (costTime > 5000)
            {
                _logger?.LogWarning(requestJson);
            }
            else
            {
                _logger?.LogInformation(requestJson);
            }
        }
    }
}