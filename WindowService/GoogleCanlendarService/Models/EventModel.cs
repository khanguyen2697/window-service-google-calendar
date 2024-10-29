using Google.Apis.Calendar.v3.Data;
using GoogleCanlendarService.Utils;
using System.Collections.Generic;
using System.Linq;

namespace GoogleCanlendarService.Models
{
    internal class EventModel : Event
    {
        public string CalendarId { get; set; }
        public string CreatorId { get; set; }
        public string OrganizerId { get; set; }
        public int? ReminderId { get; set; }
        public int? StartId { get; set; }
        public int? EndId { get; set; }
        public int? OriginalStartTimeId { get; set; }
        public bool? Deleted { get; set; }
        // Event is created from Google or not
        public bool? GoogleCreated { get; set; }
        public List<EventAttachmentModel> AttachmentsModel { get; set; }
        public List<EventAttendeeModel> AttendeesModel { get; set; }
        public CreatorDataModel CreatorModel { get; set; }
        public OrganizerDataModel OrganizerModel { get; set; }
        public RemindersDataModel RemindersModel { get; set; }
        // For Start
        public EventDateTimeModel StartEventDateTime { get; set; }
        // For OriginalStartTime
        public EventDateTimeModel OriginalStartTimeEventDateTime { get; set; }
        // For End
        public EventDateTimeModel EndEventDateTime { get; set; }

        public EventModel() { }

        public EventModel(Event eventObj, string calendarId = null)
        {
            this.Id = eventObj.Id;
            this.CalendarId = calendarId;
            this.AnyoneCanAddSelf = eventObj.AnyoneCanAddSelf;
            this.AttendeesOmitted = eventObj.AttendeesOmitted;
            this.ColorId = eventObj.ColorId;
            this.CreatedRaw = eventObj.CreatedRaw;
            this.Description = eventObj.Description;
            this.EndTimeUnspecified = eventObj.EndTimeUnspecified;
            this.ETag = eventObj.ETag;
            this.EventType = eventObj.EventType;
            this.GuestsCanInviteOthers = eventObj.GuestsCanInviteOthers;
            this.GuestsCanModify = eventObj.GuestsCanModify;
            this.GuestsCanSeeOtherGuests = eventObj.GuestsCanSeeOtherGuests;
            this.HangoutLink = eventObj.HangoutLink;
            this.HtmlLink = eventObj.HtmlLink;
            this.ICalUID = eventObj.ICalUID;
            this.Kind = eventObj.Kind;
            this.Location = eventObj.Location;
            this.Locked = eventObj.Locked;
            this.PrivateCopy = eventObj.PrivateCopy;
            this.Recurrence = eventObj.Recurrence;
            this.RecurringEventId = eventObj.RecurringEventId;
            this.Sequence = eventObj.Sequence;
            this.Status = eventObj.Status;
            this.Summary = eventObj.Summary;
            this.Transparency = eventObj.Transparency;
            this.UpdatedRaw = eventObj.UpdatedRaw;
            this.UpdatedDateTimeOffset = eventObj.UpdatedDateTimeOffset;
            this.Visibility = eventObj.Visibility;

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

            if (eventObj.Start != null)
            {
                this.StartEventDateTime = new EventDateTimeModel(eventObj.Start);
            }

            if (eventObj.OriginalStartTime != null)
            {
                this.OriginalStartTimeEventDateTime = new EventDateTimeModel(eventObj.OriginalStartTime);
            }

            if (eventObj.End != null)
            {
                this.EndEventDateTime = new EventDateTimeModel(eventObj.End);
            }

            if (eventObj.Attachments != null)
            {
                this.AttachmentsModel = new List<EventAttachmentModel>();
                EventAttachmentModel eventAttachmentModel;
                foreach (var item in eventObj.Attachments)
                {
                    eventAttachmentModel = new EventAttachmentModel(item);
                    this.AttachmentsModel.Add(eventAttachmentModel);
                }
            }

            if (eventObj.Attendees != null)
            {
                this.AttendeesModel = new List<EventAttendeeModel>();
                EventAttendeeModel eventAttendeeModel;
                foreach (var item in eventObj.Attendees)
                {
                    eventAttendeeModel = new EventAttendeeModel(item);
                    this.AttendeesModel.Add(eventAttendeeModel);
                }
            }
        }

        /// <summary>
        /// Convert from EventModel to Event
        /// </summary>
        /// <returns>Event object</returns>
        public Event ToEvent(Event googleEvent = null)
        {
            Event eventObj = new Event()
            {
                Id = this.Id,
                AnyoneCanAddSelf = this.AnyoneCanAddSelf,
                Attachments = this.Attachments,
                AttendeesOmitted = this.AttendeesOmitted,
                ColorId = this.ColorId,
                ConferenceData = this.ConferenceData,
                Creator = this.Creator,
                Description = this.Description,
                EndTimeUnspecified = this.EndTimeUnspecified,
                ETag = this.ETag,
                EventType = this.EventType,
                ExtendedProperties = this.ExtendedProperties,
                FocusTimeProperties = this.FocusTimeProperties,
                Gadget = this.Gadget,
                GuestsCanInviteOthers = this.GuestsCanInviteOthers,
                GuestsCanModify = this.GuestsCanModify,
                GuestsCanSeeOtherGuests = this.GuestsCanSeeOtherGuests,
                HangoutLink = this.HangoutLink,
                HtmlLink = this.HtmlLink,
                ICalUID = this.ICalUID,
                Kind = this.Kind,
                Location = this.Location,
                Locked = this.Locked,
                Organizer = this.Organizer,
                OutOfOfficeProperties = this.OutOfOfficeProperties,
                PrivateCopy = this.PrivateCopy,
                Recurrence = this.Recurrence,
                RecurringEventId = this.RecurringEventId,
                Reminders = this.Reminders,
                Sequence = this.Sequence,
                Source = this.Source,
                Status = this.Status,
                Summary = this.Summary,
                Transparency = this.Transparency,
                UpdatedRaw = this.UpdatedRaw,
                Visibility = this.Visibility,
                WorkingLocationProperties = this.WorkingLocationProperties
            };

            if (AttendeesModel != null)
            {
                eventObj.Attendees = AttendeesModel.ToList<EventAttendee>().ToList();
            }

            if (StartEventDateTime != null)
            {
                eventObj.Start = StartEventDateTime.ToEventDateTime();
            }

            if (EndEventDateTime != null)
            {
                eventObj.End = EndEventDateTime.ToEventDateTime();
            }

            if (OriginalStartTimeEventDateTime != null)
            {
                eventObj.OriginalStartTime = OriginalStartTimeEventDateTime.ToEventDateTime();
            }

            return eventObj;
        }

        /// <summary>
        /// Check if any simple fields changed (The field is not object, just string, int, ...)
        /// </summary>
        /// <returns>bool</returns>
        public bool AreSimpleFieldsEqual(EventModel other)
        {
            bool isFieldsEqual = false;
            if (other != null)
            {
                // Compare simple fields
                isFieldsEqual =
                    Id == other.Id
                    && CalendarId == other.CalendarId
                    && AnyoneCanAddSelf == other.AnyoneCanAddSelf
                    && AttendeesOmitted == other.AttendeesOmitted
                    && ColorId == other.ColorId
                    && CreatedRaw == other.CreatedRaw
                    && Description == other.Description
                    && EndTimeUnspecified == other.EndTimeUnspecified
                    && ETag == other.ETag
                    && EventType == other.EventType
                    && GuestsCanInviteOthers == other.GuestsCanInviteOthers
                    && GuestsCanModify == other.GuestsCanModify
                    && GuestsCanSeeOtherGuests == other.GuestsCanSeeOtherGuests
                    && HangoutLink == other.HangoutLink
                    && HtmlLink == other.HtmlLink
                    && ICalUID == other.ICalUID
                    && Kind == other.Kind
                    && Location == other.Location
                    && Locked == other.Locked
                    && PrivateCopy == other.PrivateCopy
                    && RecurringEventId == other.RecurringEventId
                    && Sequence == other.Sequence
                    && Status == other.Status
                    && Summary == other.Summary
                    && Transparency == other.Transparency
                    && UpdatedRaw == other.UpdatedRaw
                    && Visibility == other.Visibility
                    && Deleted == other.Deleted;
            }
            return isFieldsEqual;
        }

        public bool IsStartEqual(EventDateTimeModel other)
        {
            return IsEventDateTimeEqual(StartEventDateTime, other);
        }

        public bool IsEndEqual(EventDateTimeModel other)
        {
            return IsEventDateTimeEqual(EndEventDateTime, other);
        }

        public bool IsOriginalStartTimeEqual(EventDateTimeModel other)
        {
            return IsEventDateTimeEqual(OriginalStartTimeEventDateTime, other);
        }

        public bool IsAttendeesEqual(IList<EventAttendeeModel> listOther)
        {
            return CommonUtils.AreListsEqual(AttendeesModel, listOther, "Id", "EventId");
        }


        private bool IsEventDateTimeEqual(EventDateTimeModel obj1, EventDateTimeModel obj2)
        {
            if (obj1 == null && obj2 == null)
                return true;

            if (obj1 != null)
            {
                return obj1.Equals(obj2);
            }

            return obj2.Equals(obj1);
        }
    }
}
