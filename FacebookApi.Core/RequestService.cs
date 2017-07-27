using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public async Task<string> GetAsync(string urlPath, bool appendToken = true, Dictionary<string, string> queryParameters = null)
        {
            queryParameters = queryParameters ?? new Dictionary<string, string>();
            if (appendToken) queryParameters.Add("access_token", _accessToken);

            using (var client = new HttpClient { BaseAddress = new Uri(BaseUrl) })
            {
                var fullUrl = BuildUrl();
                var responseMessage = await client.GetAsync(fullUrl);
                var stringData = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new HttpRequestException(stringData);
                return stringData;
            }

            string BuildUrl()
            {
                if (queryParameters.Count == 0) return urlPath;

                var builder = new StringBuilder(urlPath).Append("?");
                var queryParametersArray = queryParameters.ToArray();
                for (var i = 0; i < queryParametersArray.Length; i++)
                {
                    if (i != 0) builder.Append("&");
                    var queryParameterKvp = queryParametersArray[i];
                    builder.Append($"{queryParameterKvp.Key}={queryParameterKvp.Value}");
                }
                return builder.ToString();
            }
        }
    }
}