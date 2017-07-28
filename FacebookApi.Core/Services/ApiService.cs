using FacebookApi.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FacebookApi.Core.Services
{
    public abstract class ApiService
    {
        protected RequestService RequestService { get; }

        public ApiService(RequestService requestService)
        {
            RequestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
        }

        public async Task<TokenInfoModel> GetLongToken(string appId, string appSecret, string userToken)
        {
            var path = $"/oauth/access_token";
            var queryParameters = new Dictionary<string, string>
            {
                ["grant_type"] = "fb_exchange_token",
                ["client_id"] = appId,
                ["client_secret"] = appSecret,
                ["fb_exchange_token"] = userToken,
            };
            var data = await RequestService.GetAsync(path, queryParameters, appendToken: false);
            var jObject = JObject.Parse(data);
            var utcNow = DateTime.UtcNow;
            var tokenInfo = new TokenInfoModel
            {
                Token = (string)jObject["access_token"],
                RequestedUtc = utcNow,
                ExpiresUtc = utcNow.AddSeconds((int)jObject["expires_in"])
            };
            return tokenInfo;
        }
    }
}