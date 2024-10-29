using Google.Apis.Calendar.v3.Data;

namespace GoogleCanlendarService.Models
{
    internal class EventAttendeeModel : EventAttendee
    {
        public string EventId { get; set; }

        public EventAttendeeModel() { }

        public EventAttendeeModel(EventAttendee eventAttendee)
        {
            Id = eventAttendee.Id;
            AdditionalGuests = eventAttendee.AdditionalGuests;
            Comment = eventAttendee.Comment;
            DisplayName = eventAttendee.DisplayName;
            Email = eventAttendee.Email;
            Optional = eventAttendee.Optional;
            Organizer = eventAttendee.Organizer;
            Resource = eventAttendee.Resource;
            ResponseStatus = eventAttendee.ResponseStatus;
            ETag = eventAttendee.ETag;
            Self = eventAttendee.Self;
        }
    }
}
