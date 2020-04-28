using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuth2Client;
using Refit;

namespace MvcClient
{

    //[Headers("Authorization: Bearer")]
    public interface IApi
    {
        [Get("/identity")]
        Task<string> GetIdentity(string scope, [Header("Authorization")]string accessToken = OAuth2ClientConst.NoneBearerToken);
    }
}
