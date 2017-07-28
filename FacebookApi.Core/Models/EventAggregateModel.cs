using System.Collections.Generic;

namespace FacebookApi.Core.Models
{
    public class EventAggregateModel
    {
        public IEnumerable<EventModel> Events { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}