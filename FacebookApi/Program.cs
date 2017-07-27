using FacebookApi.Core;
using FacebookApi.Core.Models;
using FacebookApi.Core.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookApi
{
    internal class Program
    {
        private static ApiService ApiService;
        private static ILogger Logger;
        private static IConfigurationRoot Configuration;

        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            Configuration = builder.Build();
            //var token = Configuration["UserToken"];
            var token = Configuration["AppToken"];

            ApiService = new ApiService(new RequestService(token));
            Logger = new Logger();

            GetTokenInfo().Wait();
            //GetEvents().Wait();
            //GetLongToken().Wait();
        }

        private static async Task GetTokenInfo()
        {
            var token = Configuration["UserToken"];
            await ApiService.GetTokenInfo(token);
        }

        private static async Task GetLongToken()
        {
            var appId = Configuration["AppId"];
            var appSecret = Configuration["AppSecret"];
            var token = Configuration["UserToken"];

            var tokenObj = await ApiService.GetLongToken(appId, appSecret, token);
            var json = JsonConvert.SerializeObject(tokenObj, Formatting.Indented);
        }

        private static async Task GetEvents()
        {
            var venues = new[]
            {
                "TaylorSwift",
            };
            var venueData = await venues.Select(v => CheckErrors(v)).WhenAll();
            var allEvents = venueData.SelectMany(v => v.Events).OrderBy(e => e.StartTime);
            var response = new { VenueData = allEvents, Exceptions = ((Logger)Logger).Exceptions.Select(e => e.Message) };

            var venueJson = JsonConvert.SerializeObject(venueData, Formatting.Indented);

            async Task<VenueModel> CheckErrors(string name)
            {
                try
                {
                    return await ApiService.GetEvents(name);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    return await Task.FromResult(new VenueModel());
                }
            }
        }
    }
}