using FacebookApi.Core;
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
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            var configuration = builder.Build();
            var accessToken = configuration["AppToken"];

            var apiService = new ApiService(new RequestService(accessToken));
            var logger = new Logger();
            var venues = new[]
            {
                "SycamoreBrewing",
                //"RockHouseEvents",
                "WoodenRobotBrewery",
                "219397021420603" //thomas street tavern
            };
            var venueData = venues.Select(v => CheckErrors(v)).WhenAll().Result;
            var venueJson = JsonConvert.SerializeObject(venueData);

            async Task<object> CheckErrors(string name)
            {
                try
                {
                    return await apiService.GetEvents(name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex);
                    return await Task.FromResult(default(object));
                }
            }
        }
    }
}