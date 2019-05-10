using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public class GitterAuthApi : IAuthApi
    {
        private const string APIBASE = "https://gitter.im/login/oauth/";

        private const string OAUTHKEY = "127f77459c9d8a66244863d4133d4af80d0e5c1c";
        private const string OAUTHSECRET = "ab35323b7b204c81085e398c245bd1566ecc3950";
        private const string REDIRECTURI = "https://localhost:44393/";

        private string Token { get; set; }
        private HttpClient HttpClient { get; set; }
        public GitterAuthApi(HttpClient httpClient = null)
        {
            HttpClient = httpClient ?? throw new Exception("Make sure you have added an HttpClient to your DI Container");
        }

        public async Task<string> GetAccessToken(string exchangeToken)
        {
            PrepareHttpClient();

            var result = await HttpClient.PostJsonAsync<GitterAccessToken>(
                String.Format("{0}token",
               APIBASE 
               ), new
                {
                    code = exchangeToken,
                    client_id = OAUTHKEY,
                    client_secret = OAUTHSECRET,
                    redirect_uri = REDIRECTURI,
                    grant_type = "authorization_code"
                });

            return result.Access_Token;
        }

        private void PrepareHttpClient()
        {
            if (!(HttpClient.BaseAddress is object))
            {
                HttpClient.BaseAddress = new Uri(APIBASE);
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public string GetLoginUrl()
        {
            return String.Format("{0}authorize?redirect_uri={1}&response_type={2}&client_id={3}",
               APIBASE,
               REDIRECTURI,                //redirect_uri
               "code",                     //response_type
               OAUTHKEY                    //client_id
               );
        }
    }
}
