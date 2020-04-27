using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace OAuth2Client
{
    public class DefaultAuthenticatedHttpClientHandler: AuthenticatedHttpClientHandler
    {
        public DefaultAuthenticatedHttpClientHandler(TokenRequest tokenRequest,
                                                     IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            TokenRequest = tokenRequest ?? throw new ArgumentNullException(nameof(tokenRequest));
        }

        protected override TokenRequest TokenRequest { get; }
    }
}
