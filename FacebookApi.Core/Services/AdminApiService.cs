using FacebookApi.Core.Models;
using FacebookApi.Core.Tools;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FacebookApi.Core.Services
{
    public class AdminApiService : ApiService
    {
        public AdminApiService(RequestService requestService) : base(requestService) { }

        public async Task<TokenInfoModel> GetTokenInfo(string userToken)
        {
            var path = $"debug_token";
            var queryParameters = new Dictionary<string, string> { ["input_token"] = userToken };
            var data = await RequestService.GetAsync(path, queryParameters, appendToken: true);
            var jObject = JObject.Parse(data)["data"];
            var tokenInfo = new TokenInfoModel
            {
                RequestedUtc = DotnetExtensions.FromEpochTime((int)jObject["issued_at"]),
                ExpiresUtc = DotnetExtensions.FromEpochTime((int)jObject["expires_at"]),
            };
            return tokenInfo;
        }
    }
}