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

        public async Task<Participation> GetEventAttendeesById(long eventId)
        {
            var queryParameters = new Dictionary<string, string> { ["limit"] = "100000" };
            var goingPath = $"/{eventId}/attending";
            var interestedPath = $"/{eventId}/interested";
            var goingTask = RequestService.GetAsync(goingPath, queryParameters);
            var interestedTask = RequestService.GetAsync(interestedPath, queryParameters);
            var goingData = await goingTask;
            var interestedData = await interestedTask;
            var goingJObject = JObject.Parse(goingData);
            var interestedJObject = JObject.Parse(interestedData);

            var attendingList = ParseAttendeeData(goingJObject).ToList();
            var interestedList = ParseAttendeeData(interestedJObject).ToList();
            return new Participation
            {
                Attending = attendingList,
                Interested = interestedList,
                AttendingCount = attendingList.Count,
                InterestedCount = interestedList.Count
            };

            IEnumerable<Attendee> ParseAttendeeData(JObject jData)
            {
                return jData["data"].Select(d => new Attendee
                {
                    Name = (string)d["name"],
                    Id = (string)d["id"],
                    RSVPStatus = (string)d["rsvp_status"]
                });
            }
        }

        public async Task<Participation> GetEventAttendeeCountsById(string eventId)
        {
            var interestedPath = $"{eventId}";
            var queryParameters = new Dictionary<string, string> { ["fields"] = "attending_count,declined_count,interested_count,maybe_count,noreply_count" };

            var interestedData = await RequestService.GetAsync(interestedPath, queryParameters);
            var interestedJObject = JObject.Parse(interestedData);

            return new Participation
            {
                AttendingCount = (int)interestedJObject["attending_count"],
                InterestedCount = (int)interestedJObject["interested_count"]
            };
        }

        public async Task<EventAggregateModel> GetAllEvents(params string[] venueNames)
        {
            var exceptions = new List<Exception>();
            var venueData = await venueNames.Select(v => CheckVenueErrors(v)).WhenAll();
            var allEvents = venueData.SelectMany(v => v.Events).OrderBy(e => e.StartTime).ToList();
            await CheckAttendanceErrors(allEvents);

            var eventData = new EventAggregateModel
            {
                Events = allEvents,
                Errors = exceptions.Select(e => e.Message)
            };
            return eventData;

            async Task<VenueModel> CheckVenueErrors(string name)
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

            async Task CheckAttendanceErrors(List<EventModel> events)
            {
                foreach (var @event in allEvents.Where(e => e.StartTime <= DateTime.Now.AddMonths(1)))
                {
                    try
                    {
                        var eventCounts = await GetEventAttendeeCountsById(@event.Id);
                        @event.Participation = new Participation { AttendingCount = eventCounts.AttendingCount, InterestedCount = eventCounts.InterestedCount };
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }
        }
    }
}