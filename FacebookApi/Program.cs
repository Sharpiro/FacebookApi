using FacebookApi.Core.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FacebookApi
{
    internal class Program
    {
        private static AdminApiService AdminApiService;
        private static UserApiService UserApiService;
        private static IConfigurationRoot Configuration;

        public static string AppToken { get; private set; }
        public static string UserToken { get; private set; }

        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            Configuration = builder.Build();
            AppToken = Configuration["AppToken"];
            UserToken = Configuration["UserToken"];

            AdminApiService = new AdminApiService(new RequestService(AppToken));
            UserApiService = new UserApiService(new RequestService(UserToken));

            //GetTokenInfo().Wait();
            GetEvents().Wait();
            //GetLongToken().Wait();
        }

        private static async Task GetTokenInfo()
        {
            await AdminApiService.GetTokenInfo(UserToken);
        }

        private static async Task GetLongToken()
        {
            var appId = Configuration["AppId"];
            var appSecret = Configuration["AppSecret"];

            var tokenObj = await UserApiService.GetLongToken(appId, appSecret, UserToken);
            var json = JsonConvert.SerializeObject(tokenObj, Formatting.Indented);
        }

        private static async Task GetEvents()
        {
            var venues = new[]
            {
                "TaylorSwift",
                "Bodybuildingcom"
            };
            var events = await UserApiService.GetAllEvents(venues);
            var tokenInfo = await AdminApiService.GetTokenInfo(UserToken);
            tokenInfo.Token = $"{UserToken.Substring(0, 10)}...";
            var x = new { events.Events, Token = tokenInfo, events.Errors };
        }
    }
}