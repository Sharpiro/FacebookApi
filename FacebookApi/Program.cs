using FacebookApi.Core.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FacebookApi
{
    internal class Program
    {
        private static AdminApiService _adminApiService;
        private static UserApiService _userApiService;
        private static IConfigurationRoot _configuration;

        public static string AppToken { get; private set; }
        public static string UserToken { get; private set; }

        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            _configuration = builder.Build();
            AppToken = _configuration["AppToken"];
            UserToken = _configuration["UserToken"];

            _adminApiService = new AdminApiService(new RequestService(AppToken));
            _userApiService = new UserApiService(new RequestService(UserToken));

            //GetTokenInfo().Wait();
            GetEvents().Wait();
            //GetEventById().Wait();
            //GetLongToken().Wait();
        }

        private static async Task GetEventById(string eventId)
        {
            await _userApiService.GetEventAttendeeCountsById(eventId);
        }

        private static async Task GetTokenInfo()
        {
            await _adminApiService.GetTokenInfo(UserToken);
        }

        private static async Task GetLongToken()
        {
            var appId = _configuration["AppId"];
            var appSecret = _configuration["AppSecret"];

            var tokenObj = await _userApiService.GetLongToken(appId, appSecret, UserToken);
            var json = JsonConvert.SerializeObject(tokenObj, Formatting.Indented);
        }

        private static async Task GetEvents()
        {
            var venues = new[]
            {
                "LEGO"
            };
            var events = await _userApiService.GetAllEvents(venues);
            var tokenInfo = await _adminApiService.GetTokenInfo(UserToken);
            var eventResponse = new { events.Events, Token = tokenInfo, events.Errors };
        }
    }
}