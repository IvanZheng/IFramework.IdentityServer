// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        private static async Task Main()
        {
            // discover endpoints from metadata
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret2",
                Scope = "apiall IdentityServerApi zero_identity_api"
            });
            
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            //Api.ApiManagementPermissions.Post zero.tenantRole1:f4898a97-8f4e-4f52-9102-6fdd0639dcf9
            var permissionResult = await apiClient.GetAsync("http://localhost:9001/permissions?name=Api.ApiManagementPermissions.Post&providerType=Role&providerKey=zero.tenantRole1&scopeId=3d0a1642-c2e1-4031-96a9-4fc651c245c1");
            if (!permissionResult.IsSuccessStatusCode)
            {
                Console.WriteLine(permissionResult.StatusCode);
            }
            else
            {
                var content = await permissionResult.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }

            var response = await apiClient.GetAsync("http://localhost:5001/identity?scopeId=bbb");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }

            response = await apiClient.PostAsJsonAsync("http://localhost:5001/identity", new {ScopeId = "3d0a1642-c2e1-4031-96a9-4fc651c245c1", Name = "test"});
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }
        }
    }
}