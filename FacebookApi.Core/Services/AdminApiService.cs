using FacebookApi.Core.Models;
using FacebookApi.Core.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FacebookApi.Core.Services
{
    public class AdminApiService : ApiService
    {
        public AdminApiService(RequestService requestService) : base(requestService) { }

        public async Task<TokenInfoModel> GetTokenInfo(string userToken)
        {
            if (string.IsNullOrEmpty(userToken)) throw new ArgumentNullException(nameof(userToken));

            var path = $"debug_token";
            var queryParameters = new Dictionary<string, string> { ["input_token"] = userToken };
            var data = await RequestService.GetAsync(path, queryParameters, appendToken: true);
            var jObject = JObject.Parse(data)["data"];

            var issuedAt = (int?)jObject["issued_at"];
            var tokenInfo = new TokenInfoModel
            {
                TokenPortion = $"{userToken.Substring(0, 10)}...",
                RequestedUtc = issuedAt.HasValue ? DotnetExtensions.FromEpochTime(issuedAt.Value) : default(DateTime?),
                ExpiresUtc = DotnetExtensions.FromEpochTime((int)jObject["expires_at"]),
            };
            return tokenInfo;
        }
    }
}