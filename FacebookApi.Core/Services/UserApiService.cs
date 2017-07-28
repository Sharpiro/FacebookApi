using FacebookApi.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookApi.Core.Services
{
    public class UserApiService : ApiService
    {
        public UserApiService(RequestService requestService) : base(requestService) { }

        public async Task<VenueModel> GetEventsByVenue(string venueName)
        {
            var path = $"/{venueName}/events";
            var data = await RequestService.GetAsync(path);
            var jObject = JObject.Parse(data);
            var venue = new VenueModel
            {
                Name = venueName,
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
                        VenueName = venueName
                    };
                })
                .Where(e => e.EndTime >= DateTime.Now).OrderBy(e => e.StartTime).ToList()
            };
            return venue;
        }

        public async Task<EventAggregateModel> GetAllEvents(params string[] venueNames)
        {
            var exceptions = new List<Exception>();
            var venueData = await venueNames.Select(v => CheckErrors(v)).WhenAll();
            var allEvents = venueData.SelectMany(v => v.Events).OrderBy(e => e.StartTime);
            var eventData = new EventAggregateModel
            {
                Events = allEvents,
                Errors = exceptions.Select(e => e.Message)
            };
            return eventData;

            async Task<VenueModel> CheckErrors(string name)
            {
                try
                {
                    return await GetEventsByVenue(name);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    return await Task.FromResult(new VenueModel());
                }
            }
        }
    }
}
