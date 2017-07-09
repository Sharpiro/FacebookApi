using Newtonsoft.Json.Linq;
using System;
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

        public async Task<object> GetEvents(string name)
        {
            var path = $"/{name}/events";
            var data = await _requestService.GetAsync(path);
            var jObject = JObject.Parse(data);
            var data2 = new
            {
                Venue = name,
                Events = jObject["data"].Select(e =>
                {
                    var hasEnd = DateTime.TryParse((string)e["end_time"], out DateTime x);
                    var endTime = hasEnd ? x as DateTime? : null;
                    return new
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

            //DateTime.TryParse
            return data2;
        }
    }
}