using FacebookApi.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookApi.Core
{
    public class ApiService
    {
        private readonly RequestService _requestService;

        public ApiService(RequestService requestService)
        {
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
        }

        public async Task<TokenInfoModel> GetLongToken(string appId, string appSecret, string shortLivedToken)
        {
            var path = $"/oauth/access_token?grant_type=fb_exchange_token&client_id={appId}&client_secret={appSecret}&fb_exchange_token={shortLivedToken}";
            var data = await _requestService.GetAsync(path, appendToken: false);
            var jObject = JObject.Parse(data);
            var utcNow = DateTime.UtcNow;
            var aObj = new TokenInfoModel
            {
                Token = (string)jObject["access_token"],
                RequestedUtc = utcNow,
                ExpiresUtc = utcNow.AddSeconds((int)jObject["expires_in"]),
                ExpiresSeconds = (int)jObject["expires_in"],
            };
            return aObj;
        }

        public async Task GetTokenInfo(string userToken)
        {
            var path = $"debug_token";
            var queryParameters = new Dictionary<string, string> { ["input_token"] = userToken };
            var data = await _requestService.GetAsync(path, true, queryParameters);



            throw new NotImplementedException();
        }

        public async Task<VenueModel> GetEvents(string name)
        {
            var path = $"/{name}/events";
            var data = await _requestService.GetAsync(path);
            var jObject = JObject.Parse(data);
            var data2 = new VenueModel
            {
                Name = name,
                Events = jObject["data"].Select(e =>
                {
                    var hasEnd = DateTime.TryParse((string)e["end_time"], out DateTime x);
                    var endTime = hasEnd ? x as DateTime? : null;
                    return new EventModel
                    {
                        Id = (string)e["id"],
                        Name = (string)e["name"],
                        Location = (string)e.SelectToken("place.name"),
                        Description = (string)e["description"],
                        StartTime = (DateTime)e["start_time"],
                        EndTime = endTime,
                    };
                })
                .Where(e => e.EndTime >= DateTime.Now).OrderBy(e => e.StartTime).ToList()
            };
            return data2;
        }
    }
}