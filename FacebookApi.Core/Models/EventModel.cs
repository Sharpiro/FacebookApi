using System;
using System.Collections.Generic;

namespace FacebookApi.Core.Models
{
    public class EventModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string VenueName { get; set; }
        public Participation Participation { get; set; }
    }

    public class Participation
    {
        public List<Attendee> Attending { get; set; } = new List<Attendee>();
        public List<Attendee> Interested { get; set; } = new List<Attendee>();
        public int AttendingCount { get; set; }
        public int InterestedCount { get; set; }
    }

    public class Attendee
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string RSVPStatus { get; set; }
    }
}