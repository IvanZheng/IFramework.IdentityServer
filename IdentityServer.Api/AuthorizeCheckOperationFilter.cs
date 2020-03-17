using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace IdentityServer.Api
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        private readonly AdminApiConfiguration _adminApiConfiguration;

        public AuthorizeCheckOperationFilter(AdminApiConfiguration adminApiConfiguration)
        {
            _adminApiConfiguration = adminApiConfiguration;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            // Policy names map to scopes
            var requiredScopes = context.MethodInfo
                                        .GetCustomAttributes(true)
                                        .OfType<AuthorizeAttribute>()
                                        .Select(attr => attr.Policy)
                                        .Distinct()
                                        .ToList();

            if (requiredScopes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                };
                
                requiredScopes.Add(_adminApiConfiguration.OidcApiName);
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [ oAuthScheme ] = requiredScopes
                    }
                };
            }
        }
    }
}
