using Google.Apis.Calendar.v3.Data;
using GoogleCanlendarService.Models;
using GoogleCanlendarService.Repository;
using GoogleCanlendarService.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GoogleCanlendarService.Data
{
    class EventRepository : BaseRepository
    {
        public EventRepository(SqlConnection connection, SqlTransaction transaction = null)
            : base(connection, transaction) { }


        public void Insert(Event eventObj)
        {
            string StrQuery =
                @"INSERT INTO Event (
                    Id,
                    AnyoneCanAddSelf,
                    AttendeesOmitted,
                    CreatedRaw,
                    UpdatedRaw,
                    ColorId,
                    CreatedDateTimeOffset,
                    CreatorId,
                    EDescription,
                    EndDate,
                    EndTimeUnspecified,
                    ETag,
                    EventType,
                    GuestsCanInviteOthers,
                    GuestsCanModify,
                    GuestsCanSeeOtherGuests,
                    HangoutLink,
                    HtmlLink,
                    ICalUID,
                    Location,
                    Kind,
                    Locked,
                    OrganizerId,
                    PrivateCopy,
                    OriginalStartTime,
                    RecurringEventId,
                    ReminderId,
                    StartDate,
                    EStatus,
                    Summary,
                    Transparency,
                    Visibility
                ) VALUES (
                    @Id,
                    @AnyoneCanAddSelf,
                    @AttendeesOmitted,
                    @CreatedRaw,
                    @UpdatedRaw,
                    @ColorId,
                    @CreatedDateTimeOffset,
                    @CreatorId,
                    @EDescription,
                    @EndDate,
                    @EndTimeUnspecified,
                    @ETag,
                    @EventType,
                    @GuestsCanInviteOthers,
                    @GuestsCanModify,
                    @GuestsCanSeeOtherGuests,
                    @HangoutLink,
                    @HtmlLink,
                    @ICalUID,
                    @Location,
                    @Kind,
                    @Locked,
                    @OrganizerId,
                    @PrivateCopy,
                    @OriginalStartTime,
                    @RecurringEventId,
                    @ReminderId,
                    @StartDate,
                    @EStatus,
                    @Summary,
                    @Transparency,
                    @Visibility
                );";

            // This code is for testing purposes only
            //eventObj.Attachments = DumpData.EventAttachmentList();
            //eventObj.Attendees = DumpData.EventAttendeeList();

            EventModel eventModel = new EventModel(eventObj);
            string newCreatorId = null;
            string newOrganizerId = null;
            int? newReminderId = null;
            try
            {
                this.Transaction = this.Connection.BeginTransaction();
                // Insert CreatorData
                if (eventModel.CreatorModel != null)
                {
                    // Using EventId incase creatorId is null
                    if (eventModel.CreatorModel.Id == null)
                    {
                        eventModel.CreatorModel.Id = eventModel.Id;
                    }
                    CreatorDataRepository creatorDataRepo = new CreatorDataRepository(this.Connection, this.Transaction);
                    newCreatorId = creatorDataRepo.Insert(eventModel.CreatorModel);
                }

                // Insert OrganizerData
                if (eventModel.OrganizerModel != null)
                {
                    // Using EventId incase OrganizerId is null
                    if (eventModel.OrganizerModel.Id == null)
                    {
                        eventModel.OrganizerModel.Id = eventModel.Id;
                    }
                    OrganizerDataRepository organizerDataRepository = new OrganizerDataRepository(this.Connection, this.Transaction);
                    newOrganizerId = organizerDataRepository.Insert(eventModel.OrganizerModel);
                }

                // Insert RemindersData
                if (eventModel.RemindersModel != null)
                {
                    RemindersDataRepository remindersDataRepo = new RemindersDataRepository(this.Connection, this.Transaction);
                    newReminderId = remindersDataRepo.Insert(eventModel.RemindersModel);
                }

                // Insert Event
                using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection, this.Transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", DBUtils.GetDBValue(eventModel.Id));
                    cmd.Parameters.AddWithValue("@AnyoneCanAddSelf", DBUtils.GetDBValue(eventModel.AnyoneCanAddSelf));
                    cmd.Parameters.AddWithValue("@AttendeesOmitted", DBUtils.GetDBValue(eventModel.AttendeesOmitted));
                    cmd.Parameters.AddWithValue("@CreatedRaw", DBUtils.GetDBValue(eventModel.CreatedRaw));
                    cmd.Parameters.AddWithValue("@ColorId", DBUtils.GetDBValue(eventModel.ColorId));
                    cmd.Parameters.AddWithValue("@CreatedDateTimeOffset", DBUtils.GetDBValue(eventModel.CreatedDateTimeOffset));
                    cmd.Parameters.AddWithValue("@CreatorId", DBUtils.GetDBValue(newCreatorId));
                    cmd.Parameters.AddWithValue("@EDescription", DBUtils.GetDBValue(eventModel.Description));
                    cmd.Parameters.AddWithValue("@EndDate", DBUtils.GetDBValue(eventModel.End?.DateTimeDateTimeOffset));
                    cmd.Parameters.AddWithValue("@EndTimeUnspecified", DBUtils.GetDBValue(eventModel.EndTimeUnspecified));
                    cmd.Parameters.AddWithValue("@ETag", DBUtils.GetDBValue(eventModel.ETag));
                    cmd.Parameters.AddWithValue("@EventType", DBUtils.GetDBValue(eventModel.EventType));
                    cmd.Parameters.AddWithValue("@GuestsCanInviteOthers", DBUtils.GetDBValue(eventModel.GuestsCanInviteOthers));
                    cmd.Parameters.AddWithValue("@GuestsCanModify", DBUtils.GetDBValue(eventModel.GuestsCanModify));
                    cmd.Parameters.AddWithValue("@GuestsCanSeeOtherGuests", DBUtils.GetDBValue(eventModel.GuestsCanSeeOtherGuests));
                    cmd.Parameters.AddWithValue("@HangoutLink", DBUtils.GetDBValue(eventModel.HangoutLink));
                    cmd.Parameters.AddWithValue("@HtmlLink", DBUtils.GetDBValue(eventModel.HtmlLink));
                    cmd.Parameters.AddWithValue("@ICalUID", DBUtils.GetDBValue(eventModel.ICalUID?.ToString()));
                    cmd.Parameters.AddWithValue("@Location", DBUtils.GetDBValue(eventModel.Location));
                    cmd.Parameters.AddWithValue("@Kind", DBUtils.GetDBValue(eventModel.Kind));
                    cmd.Parameters.AddWithValue("@Locked", DBUtils.GetDBValue(eventModel.Locked));
                    cmd.Parameters.AddWithValue("@OrganizerId", DBUtils.GetDBValue(newOrganizerId));
                    cmd.Parameters.AddWithValue("@PrivateCopy", DBUtils.GetDBValue(eventModel.PrivateCopy));
                    cmd.Parameters.AddWithValue("@OriginalStartTime", DBUtils.GetDBValue(eventModel.OriginalStartTime?.DateTimeDateTimeOffset));
                    cmd.Parameters.AddWithValue("@RecurringEventId", DBUtils.GetDBValue(eventModel.RecurringEventId));
                    cmd.Parameters.AddWithValue("@ReminderId", DBUtils.GetDBValue(newReminderId));
                    cmd.Parameters.AddWithValue("@StartDate", DBUtils.GetDBValue(eventModel.Start?.DateTimeDateTimeOffset));
                    cmd.Parameters.AddWithValue("@EStatus", DBUtils.GetDBValue(eventModel.Status));
                    cmd.Parameters.AddWithValue("@Summary", DBUtils.GetDBValue(eventModel.Summary));
                    cmd.Parameters.AddWithValue("@Transparency", DBUtils.GetDBValue(eventModel.Transparency));
                    cmd.Parameters.AddWithValue("@UpdatedRaw", DBUtils.GetDBValue(eventModel.UpdatedRaw));
                    cmd.Parameters.AddWithValue("@Visibility", DBUtils.GetDBValue(eventModel.Visibility));

                    cmd.ExecuteNonQuery();
                }

                // Insert EventAttachment
                if (eventModel.AttachmentsModel != null)
                {
                    EventAttachmentRepository eventAttachmentRepo = new EventAttachmentRepository(this.Connection, this.Transaction);
                    foreach (var attachment in eventModel.AttachmentsModel)
                    {
                        attachment.EventId = eventObj.Id;
                        eventAttachmentRepo.Insert(attachment);
                    }
                }

                // Insert EventAttendee
                if (eventModel.AttendeesModel != null)
                {
                    EventAttendeeRepository eventAttendeeRepo = new EventAttendeeRepository(this.Connection, this.Transaction);
                    foreach (var attendee in eventModel.AttendeesModel)
                    {
                        attendee.EventId = eventObj.Id;
                        eventAttendeeRepo.Insert(attendee);
                    }
                }

                this.Transaction.Commit();
            }
            catch (Exception ex)
            {
                this.Transaction.Rollback();
                throw ex;
            }
        }

        public List<string> GetUpcomingEventIds()
        {
            List<string> eventIds = new List<string>();
            string StrQuery = "SELECT Id FROM Event WHERE StartDate > @CurrentTime";

            using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection))
            {
                cmd.Parameters.AddWithValue("@CurrentTime", DateTime.Now);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        eventIds.Add(reader["Id"].ToString());
                    }
                }
            }

            return eventIds;
        }
    }
}
