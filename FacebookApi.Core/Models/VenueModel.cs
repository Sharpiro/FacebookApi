using System.Collections.Generic;

namespace FacebookApi.Core.Models
{
    public class VenueModel
    {
        public string Name { get; set; }
        public IEnumerable<EventModel> Events { get; set; } = new List<EventModel>();
    }
}