using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace OAuth2Client
{
    public static class OAuth2ClientServiceExtension
    {
        public static IHttpClientBuilder AddOAuth2Client<TService, TAuthenticatedHttpClientHandler>(this IServiceCollection services,
                                                                                                    RefitSettings settings = null)
            where TAuthenticatedHttpClientHandler: AuthenticatedHttpClientHandler
            where TService : class
        {
            return services.AddSingleton<TAuthenticatedHttpClientHandler>()
                           .AddRefitClient<TService>(settings)
                           .AddHttpMessageHandler<TAuthenticatedHttpClientHandler>();
        }

        public static IHttpClientBuilder AddOAuth2Client<TService>(this IServiceCollection services,
                                                                   TokenRequest tokenRequest,
                                                                   RefitSettings settings = null)
            where TService: class

        {
            return services.AddOAuth2Client<TService>(p => tokenRequest, settings);
        }

        public static IHttpClientBuilder AddOAuth2Client<TService>(this IServiceCollection services,
                                                                   Func<IServiceProvider, TokenRequest> getTokenRequest,
                                                                   RefitSettings settings = null)
        where TService: class
        {
            return services.AddRefitClient<TService>(settings)
                           .AddHttpMessageHandler(p =>
                           {
                               var tokenRequest = getTokenRequest(p);
                               return new DefaultAuthenticatedHttpClientHandler(tokenRequest,
                                                                                p.GetService<IHttpClientFactory>());
                           });
        }

    }
}
