using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FacebookApi.Core
{
    public class RequestService
    {
        private const string BaseUrl = "https://graph.facebook.com";
        private readonly string _accessToken;

        public RequestService(string accessToken)
        {
            _accessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        }

        public async Task<string> GetAsync(string urlPath)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(BaseUrl) })
            {
                var withOptions = $"{urlPath}?access_token={_accessToken}";
                var responseMessage = await client.GetAsync(withOptions);
                var stringData = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new HttpRequestException(stringData);
                return stringData;
            }
        }
    }
}