using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace OAuth2Client
{
    public static class RefitServiceResultExtension
    {
        public static async Task ProcessApiException(this Task task)
        {
            try
            { 
                await task.ConfigureAwait(false);
            }
            catch (ApiException e)
            {
                throw new Exception(e.Content);
            }
        }
        public static async Task<T> ProcessApiException<T>(this Task<T> task)
        {
            try
            {
                return await task.ConfigureAwait(false);
            }
            catch (ApiException e)
            {
                throw new Exception(e.Content);
            }
        }
    }
}
