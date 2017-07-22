using FacebookApi.Core;
using FacebookApi.Core.Models;
using FacebookApi.Core.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
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
            GetAssemblyInfo();

            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            Configuration = builder.Build();
            var token = Configuration["UserToken"];
            //var token = Configuration["AppToken"];

            ApiService = new ApiService(new RequestService(token));
            Logger = new Logger();

            GetEvents().Wait();
            //GetLongToken().Wait();
        }

        private static void GetAssemblyInfo()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var assemblyVersion = entryAssembly.GetName().Version;

            var fvi = FileVersionInfo.GetVersionInfo(entryAssembly.Location);
            string version = fvi.FileVersion;
        }

        private static async Task GetLongToken()
        {
            var appId = Configuration["AppId"];
            var appSecret = Configuration["AppSecret"];
            //var token = Configuration["AppToken"];
            var token = Configuration["UserToken"];

            var tokenObj = await ApiService.GetLongToken(appId, appSecret, token);
            var json = JsonConvert.SerializeObject(tokenObj);
        }

        private static async Task GetEvents()
        {
            var venues = new[]
            {
                "triplecbrewing",
                "SycamoreBrewing",
                "RockHouseEvents",
                "WoodenRobotBrewery",
                "219397021420603", //thomas street tavern
                "OldeMeckBrewery",
                "WoodenRobotBrewery"
            };
            var venueData = await venues.Select(v => CheckErrors(v)).WhenAll();
            var allEvents = venueData.SelectMany(e => e.Events).OrderBy(e => e.StartTime);
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
                    return await Task.FromResult(default(VenueModel));
                }
            }
        }
    }
}