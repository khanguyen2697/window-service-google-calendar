using Google.Apis.Calendar.v3.Data;
using System.Collections.Generic;

namespace GoogleCanlendarService.Models
{
    class EventModel : Event
    {
        public List<EventAttachmentModel> AttachmentsModel { get; set; }
        public List<EventAttendeeModel> AttendeesModel { get; set; }
        public CreatorDataModel CreatorModel { get; set; }
        public OrganizerDataModel OrganizerModel { get; set; }
        public RemindersDataModel RemindersModel { get; set; }

        public EventModel() { }

        public EventModel(Event eventObj)
        {
            this.Id = eventObj.Id;
            this.AnyoneCanAddSelf = eventObj.AnyoneCanAddSelf;
            this.AttendeesOmitted = eventObj.AttendeesOmitted;
            this.ColorId = eventObj.ColorId;
            this.ConferenceData = eventObj.ConferenceData;
            this.CreatedRaw = eventObj.CreatedRaw;
            this.CreatedDateTimeOffset = eventObj.CreatedDateTimeOffset;
            this.Description = eventObj.Description;
            this.End = eventObj.End;
            this.EndTimeUnspecified = eventObj.EndTimeUnspecified;
            this.ETag = eventObj.ETag;
            this.EventType = eventObj.EventType;
            this.ExtendedProperties = eventObj.ExtendedProperties;
            this.FocusTimeProperties = eventObj.FocusTimeProperties;
            this.Gadget = eventObj.Gadget;
            this.GuestsCanInviteOthers = eventObj.GuestsCanInviteOthers;
            this.GuestsCanModify = eventObj.GuestsCanModify;
            this.GuestsCanSeeOtherGuests = eventObj.GuestsCanSeeOtherGuests;
            this.HangoutLink = eventObj.HangoutLink;
            this.HtmlLink = eventObj.HtmlLink;
            this.ICalUID = eventObj.ICalUID;
            this.Kind = eventObj.Kind;
            this.Location = eventObj.Location;
            this.Locked = eventObj.Locked;
            this.OriginalStartTime = eventObj.OriginalStartTime;
            this.OutOfOfficeProperties = eventObj.OutOfOfficeProperties;
            this.PrivateCopy = eventObj.PrivateCopy;
            this.Recurrence = eventObj.Recurrence;
            this.RecurringEventId = eventObj.RecurringEventId;
            this.Sequence = eventObj.Sequence;
            this.Source = eventObj.Source;
            this.Start = eventObj.Start;
            this.Status = eventObj.Status;
            this.Summary = eventObj.Summary;
            this.Transparency = eventObj.Transparency;
            this.UpdatedRaw = eventObj.UpdatedRaw;
            this.UpdatedDateTimeOffset = eventObj.UpdatedDateTimeOffset;
            this.Visibility = eventObj.Visibility;
            this.WorkingLocationProperties = eventObj.WorkingLocationProperties;
            this.AttachmentsModel = new List<EventAttachmentModel>();
            this.AttendeesModel = new List<EventAttendeeModel>();

            if (eventObj.Creator != null)
            {
                this.CreatorModel = new CreatorDataModel(eventObj.Creator);
            }

            if (eventObj.Organizer != null)
            {
                this.OrganizerModel = new OrganizerDataModel(eventObj.Organizer);
            }

            if (eventObj.Reminders != null)
            {
                this.RemindersModel = new RemindersDataModel(eventObj.Reminders);
            }

            if (eventObj.Attachments != null)
            {
                EventAttachmentModel eventAttachmentModel;
                foreach (var item in eventObj.Attachments)
                {
                    eventAttachmentModel = new EventAttachmentModel(item);
                    this.AttachmentsModel.Add(eventAttachmentModel);
                }
            }

            if (eventObj.Attendees != null)
            {
                EventAttendeeModel eventAttendeeModel;
                foreach (var item in eventObj.Attendees)
                {
                    eventAttendeeModel = new EventAttendeeModel(item);
                    this.AttendeesModel.Add(eventAttendeeModel);
                }
            }

        }
    }
}
