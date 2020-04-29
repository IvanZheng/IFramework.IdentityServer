﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace OAuth2Client
{
    public static class OAuth2ClientServiceExtension
    {
        public static IHttpClientBuilder AddOAuth2Client<TService, TAuthenticatedHttpClientHandler>(this IServiceCollection services,
                                                                                                    string address,
                                                                                                    RefitSettings settings = null)
            where TAuthenticatedHttpClientHandler : AuthenticatedHttpClientHandler
            where TService : class
        {
            return services.AddSingleton<TAuthenticatedHttpClientHandler>()
                           .AddRefitClient<TService>(settings)
                           .ConfigureHttpClient(c => c.BaseAddress = new Uri(address))
                           .AddHttpMessageHandler<TAuthenticatedHttpClientHandler>();
        }

        public static IHttpClientBuilder AddOAuth2Client<TService>(this IServiceCollection services,
                                                                   string address,
                                                                   TokenRequest tokenRequest,
                                                                   int refreshTokenBeforeTotalSeconds = 60,
                                                                   RefitSettings settings = null)
            where TService : class

        {
            return services.AddOAuth2Client<TService>(address, p => tokenRequest, refreshTokenBeforeTotalSeconds, settings);
        }


        public static IHttpClientBuilder AddOAuth2Client<TService>(this IServiceCollection services,
                                                                   Action<IServiceProvider, HttpClient> configHttpClient,
                                                                   Func<IServiceProvider, TokenRequest> getTokenRequest,
                                                                   int refreshTokenBeforeTotalSeconds = 60,
                                                                   RefitSettings settings = null)
            where TService : class
        {
            return services.AddRefitClient<TService>(settings)
                           .ConfigureHttpClient(configHttpClient)
                           .AddHttpMessageHandler(p =>
                           {
                               var tokenRequest = getTokenRequest(p);
                               return new DefaultAuthenticatedHttpClientHandler(tokenRequest,
                                                                                p.GetService<IHttpClientFactory>(),
                                                                                refreshTokenBeforeTotalSeconds);
                           });
        }

        public static IHttpClientBuilder AddOAuth2Client<TService>(this IServiceCollection services,
                                                                   string address,
                                                                   Func<IServiceProvider, TokenRequest> getTokenRequest,
                                                                   int refreshTokenBeforeTotalSeconds = 60,
                                                                   RefitSettings settings = null)
            where TService : class
        {
            return services.AddRefitClient<TService>(settings)
                           .ConfigureHttpClient((p, c) => c.BaseAddress = new Uri(address))
                           .AddHttpMessageHandler(p =>
                           {
                               var tokenRequest = getTokenRequest(p);
                               return new DefaultAuthenticatedHttpClientHandler(tokenRequest,
                                                                                p.GetService<IHttpClientFactory>(),
                                                                                refreshTokenBeforeTotalSeconds);
                           });
        }

        public static IHttpClientBuilder AddClientCredentialsClient<TService>(this IServiceCollection services,
                                                                              string address,
                                                                              string tokenEndpoint,
                                                                              string clientId,
                                                                              string clientSecret,
                                                                              string scope,
                                                                              int refreshTokenBeforeTotalSeconds = 60,
                                                                              IDictionary<string, string> parameters = null,
                                                                              RefitSettings settings = null)
            where TService : class
        {
            return services.AddOAuth2Client<TService>(address,
                                                      p => new ClientCredentialsTokenRequest
                                                      {
                                                          Address = tokenEndpoint,
                                                          ClientId = clientId,
                                                          ClientSecret = clientSecret,
                                                          Scope = scope,
                                                          Parameters = parameters
                                                      },
                                                      refreshTokenBeforeTotalSeconds,
                                                      settings);
        }
    }
}