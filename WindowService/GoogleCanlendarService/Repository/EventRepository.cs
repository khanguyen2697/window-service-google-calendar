using Google.Apis.Calendar.v3.Data;
using GoogleCanlendarService.Models;
using GoogleCanlendarService.Repository;
using GoogleCanlendarService.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GoogleCanlendarService.Data
{
    class EventRepository : BaseRepository
    {
        public EventRepository(SqlConnection connection, SqlTransaction transaction = null)
            : base(connection, transaction) { }

        public void Insert(Event eventObj, string calendarId)
        {
            string StrQuery =
                @"INSERT INTO Event (
                    Id,
                    CalendarId,
                    ColorId,
                    CreatorId,
                    OrganizerId,
                    RecurringEventId,
                    ReminderId,
                    StartId,
                    EndId,
                    OriginalStartTimeId,
                    Sequence,
                    AnyoneCanAddSelf,
                    AttendeesOmitted,
                    CreatedRaw,
                    UpdatedRaw,
                    EDescription,
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
                    PrivateCopy,
                    EStatus,
                    Summary,
                    Transparency,
                    Visibility,
                    GoogleCreated
                ) VALUES (
                    @Id,
                    @CalendarId,
                    @ColorId,
                    @CreatorId,
                    @OrganizerId,
                    @RecurringEventId,
                    @ReminderId,
                    @StarId,
                    @EndId,
                    @OriginalStartTimeId,
                    @Sequence,
                    @AnyoneCanAddSelf,
                    @AttendeesOmitted,
                    @CreatedRaw,
                    @UpdatedRaw,
                    @EDescription,
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
                    @PrivateCopy,
                    @EStatus,
                    @Summary,
                    @Transparency,
                    @Visibility,
                    1
                );";

            EventModel eventModel = new EventModel(eventObj);
            string newCreatorId = null;
            string newOrganizerId = null;
            int? newReminderId = null;
            int? newStartId = null;
            int? newEndId = null;
            int? newOriginalStartTimeId = null;
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

                // Insert Start/OriginalStartTime/End
                if (eventModel.StartEventDateTime != null || eventModel.OriginalStartTimeEventDateTime != null || eventModel.EndEventDateTime != null)
                {
                    EventDateTimeRepository eventDateTimeRepository = new EventDateTimeRepository(this.Connection, this.Transaction);
                    if (eventModel.StartEventDateTime != null)
                    {
                        newStartId = eventDateTimeRepository.Insert(eventModel.StartEventDateTime);
                    }
                    if (eventModel.OriginalStartTimeEventDateTime != null)
                    {
                        newOriginalStartTimeId = eventDateTimeRepository.Insert(eventModel.OriginalStartTimeEventDateTime);
                    }
                    if (eventModel.EndEventDateTime != null)
                    {
                        newEndId = eventDateTimeRepository.Insert(eventModel.EndEventDateTime);
                    }
                }

                // Insert Event
                using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection, this.Transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", DBUtils.GetDBValue(eventModel.Id));
                    cmd.Parameters.AddWithValue("@CalendarId", calendarId);
                    cmd.Parameters.AddWithValue("@ColorId", DBUtils.GetDBValue(eventModel.ColorId));
                    cmd.Parameters.AddWithValue("@CreatorId", DBUtils.GetDBValue(newCreatorId));
                    cmd.Parameters.AddWithValue("@OrganizerId", DBUtils.GetDBValue(newOrganizerId));
                    cmd.Parameters.AddWithValue("@RecurringEventId", DBUtils.GetDBValue(eventModel.RecurringEventId));
                    cmd.Parameters.AddWithValue("@ReminderId", DBUtils.GetDBValue(newReminderId));
                    cmd.Parameters.AddWithValue("@StarId", DBUtils.GetDBValue(newStartId));
                    cmd.Parameters.AddWithValue("@EndId", DBUtils.GetDBValue(newEndId));
                    cmd.Parameters.AddWithValue("@OriginalStartTimeId", DBUtils.GetDBValue(newOriginalStartTimeId));
                    cmd.Parameters.AddWithValue("@Sequence", DBUtils.GetDBValue(eventModel.Sequence));
                    cmd.Parameters.AddWithValue("@AnyoneCanAddSelf", DBUtils.GetDBValue(eventModel.AnyoneCanAddSelf));
                    cmd.Parameters.AddWithValue("@AttendeesOmitted", DBUtils.GetDBValue(eventModel.AttendeesOmitted));
                    cmd.Parameters.AddWithValue("@CreatedRaw", DBUtils.GetDBValue(eventModel.CreatedRaw));
                    cmd.Parameters.AddWithValue("@UpdatedRaw", DBUtils.GetDBValue(eventModel.UpdatedRaw));
                    cmd.Parameters.AddWithValue("@EDescription", DBUtils.GetDBValue(eventModel.Description));
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
                    cmd.Parameters.AddWithValue("@PrivateCopy", DBUtils.GetDBValue(eventModel.PrivateCopy));
                    cmd.Parameters.AddWithValue("@EStatus", DBUtils.GetDBValue(eventModel.Status));
                    cmd.Parameters.AddWithValue("@Summary", DBUtils.GetDBValue(eventModel.Summary));
                    cmd.Parameters.AddWithValue("@Transparency", DBUtils.GetDBValue(eventModel.Transparency));
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

                // Insert Recurrence
                if (eventModel.Recurrence != null)
                {
                    RecurrenceRepository recurrenceRepository = new RecurrenceRepository(this.Connection, this.Transaction);
                    recurrenceRepository.InsertList(eventModel.Recurrence, eventModel.Id);
                }

                this.Transaction.Commit();
            }
            catch (Exception ex)
            {
                this.Transaction.Rollback();
                throw ex;
            }
        }

        public void UpdateSimpleField(EventModel eventObj)
        {
            string StrQuery =
                @"UPDATE 
                    Event
                SET 
                    CalendarId = @CalendarId,
                    ColorId = @ColorId,
                    CreatorId = @CreatorId,
                    OrganizerId = @OrganizerId,
                    RecurringEventId = @RecurringEventId,
                    ReminderId = @ReminderId,
                    OriginalStartTimeId = @OriginalStartTimeId,
                    Sequence = @Sequence,
                    AnyoneCanAddSelf = @AnyoneCanAddSelf,
                    AttendeesOmitted = @AttendeesOmitted,
                    UpdatedRaw = @UpdatedRaw,
                    EDescription = @EDescription,
                    EndTimeUnspecified = @EndTimeUnspecified,
                    ETag = @ETag,
                    EventType = @EventType,
                    GuestsCanInviteOthers = @GuestsCanInviteOthers,
                    GuestsCanModify = @GuestsCanModify,
                    GuestsCanSeeOtherGuests = @GuestsCanSeeOtherGuests,
                    HangoutLink = @HangoutLink,
                    HtmlLink = @HtmlLink,
                    ICalUID = @ICalUID,
                    Location = @Location,
                    Kind = @Kind,
                    Locked = @Locked,
                    PrivateCopy = @PrivateCopy,
                    EStatus = @EStatus,
                    Summary = @Summary,
                    Transparency = @Transparency,
                    Visibility = @Visibility,
                    Deleted = 0
                WHERE
                    Id = @Id";

            try
            {
                using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection, this.Transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", DBUtils.GetDBValue(eventObj.Id));
                    cmd.Parameters.AddWithValue("@CalendarId", eventObj.CalendarId);
                    cmd.Parameters.AddWithValue("@ColorId", DBUtils.GetDBValue(eventObj.ColorId));
                    cmd.Parameters.AddWithValue("@CreatorId", DBUtils.GetDBValue(eventObj.CreatorId));
                    cmd.Parameters.AddWithValue("@OrganizerId", DBUtils.GetDBValue(eventObj.OrganizerId));
                    cmd.Parameters.AddWithValue("@RecurringEventId", DBUtils.GetDBValue(eventObj.RecurringEventId));
                    cmd.Parameters.AddWithValue("@ReminderId", DBUtils.GetDBValue(eventObj.ReminderId));
                    cmd.Parameters.AddWithValue("@StartId", DBUtils.GetDBValue(eventObj.StartId));
                    cmd.Parameters.AddWithValue("@EndId", DBUtils.GetDBValue(eventObj.EndId));
                    cmd.Parameters.AddWithValue("@OriginalStartTimeId", DBUtils.GetDBValue(eventObj.OriginalStartTimeId));
                    cmd.Parameters.AddWithValue("@Sequence", DBUtils.GetDBValue(eventObj.Sequence));
                    cmd.Parameters.AddWithValue("@AnyoneCanAddSelf", DBUtils.GetDBValue(eventObj.AnyoneCanAddSelf));
                    cmd.Parameters.AddWithValue("@AttendeesOmitted", DBUtils.GetDBValue(eventObj.AttendeesOmitted));
                    cmd.Parameters.AddWithValue("@UpdatedRaw", DBUtils.GetDBValue(eventObj.UpdatedRaw));
                    cmd.Parameters.AddWithValue("@EDescription", DBUtils.GetDBValue(eventObj.Description));
                    cmd.Parameters.AddWithValue("@EndTimeUnspecified", DBUtils.GetDBValue(eventObj.EndTimeUnspecified));
                    cmd.Parameters.AddWithValue("@ETag", DBUtils.GetDBValue(eventObj.ETag));
                    cmd.Parameters.AddWithValue("@EventType", DBUtils.GetDBValue(eventObj.EventType));
                    cmd.Parameters.AddWithValue("@GuestsCanInviteOthers", DBUtils.GetDBValue(eventObj.GuestsCanInviteOthers));
                    cmd.Parameters.AddWithValue("@GuestsCanModify", DBUtils.GetDBValue(eventObj.GuestsCanModify));
                    cmd.Parameters.AddWithValue("@GuestsCanSeeOtherGuests", DBUtils.GetDBValue(eventObj.GuestsCanSeeOtherGuests));
                    cmd.Parameters.AddWithValue("@HangoutLink", DBUtils.GetDBValue(eventObj.HangoutLink));
                    cmd.Parameters.AddWithValue("@HtmlLink", DBUtils.GetDBValue(eventObj.HtmlLink));
                    cmd.Parameters.AddWithValue("@ICalUID", DBUtils.GetDBValue(eventObj.ICalUID?.ToString()));
                    cmd.Parameters.AddWithValue("@Location", DBUtils.GetDBValue(eventObj.Location));
                    cmd.Parameters.AddWithValue("@Kind", DBUtils.GetDBValue(eventObj.Kind));
                    cmd.Parameters.AddWithValue("@Locked", DBUtils.GetDBValue(eventObj.Locked));
                    cmd.Parameters.AddWithValue("@PrivateCopy", DBUtils.GetDBValue(eventObj.PrivateCopy));
                    cmd.Parameters.AddWithValue("@EStatus", DBUtils.GetDBValue(eventObj.Status));
                    cmd.Parameters.AddWithValue("@Summary", DBUtils.GetDBValue(eventObj.Summary));
                    cmd.Parameters.AddWithValue("@Transparency", DBUtils.GetDBValue(eventObj.Transparency));
                    cmd.Parameters.AddWithValue("@Visibility", DBUtils.GetDBValue(eventObj.Visibility));

                    cmd.ExecuteNonQuery();
                }
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
            string StrQuery = @"
                SELECT
                    Event.Id
                FROM
                    Event
                LEFT JOIN
                    EventDateTime
                ON
                    Event.StartId = EventDateTime.Id
                WHERE
                    TRY_CAST(EventDateTime.DateTimeRaw AS DATETIMEOFFSET) > SYSDATETIMEOFFSET()";

            using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection))
            {
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

        public bool DeleteById(string id, string updatedRaw = null)
        {
            string strQuery = @"
                UPDATE
                    Event 
                SET
                    Deleted = 1,
                    UpdatedRaw = @UpdatedRaw
                WHERE
                    Id = @Id";
            bool isDeleted = false;
            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(strQuery, this.Connection)
                : new SqlCommand(strQuery, this.Connection, this.Transaction))
            {
                if (string.IsNullOrEmpty(updatedRaw))
                {
                    updatedRaw = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                }
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UpdatedRaw", updatedRaw);
                int rowsAffected = cmd.ExecuteNonQuery();
                isDeleted = rowsAffected > 0;
            }
            return isDeleted;
        }


        /// <summary>
        /// Update Event in database after handle event on Google
        /// </summary>
        /// <param name="eventUpdatedFromGoogle">Calendar identifier. Default is "primary"</param>
        public bool UpdateEventAfterHandleOnGoogle(Event eventUpdatedFromGoogle)
        {
            string strQuery = @"
                UPDATE
                    Event 
                SET
                    ETag = @ETag,
                    Sequence = @Sequence,
                    UpdatedRaw = @UpdatedRaw
                WHERE
                    Id = @Id";
            bool isUpdated = false;
            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(strQuery, this.Connection)
                : new SqlCommand(strQuery, this.Connection, this.Transaction))
            {
                cmd.Parameters.AddWithValue("@Id", eventUpdatedFromGoogle.Id);
                cmd.Parameters.AddWithValue("@ETag", eventUpdatedFromGoogle.ETag);
                cmd.Parameters.AddWithValue("@Sequence", eventUpdatedFromGoogle.Sequence);
                cmd.Parameters.AddWithValue("@UpdatedRaw", eventUpdatedFromGoogle.UpdatedRaw);
                int rowsAffected = cmd.ExecuteNonQuery();
                isUpdated = rowsAffected > 0;
            }
            return isUpdated;
        }

        public IList<EventModel> GetAllUpcommingEventsDiffFromDB(IList<Event> events, string calendarId)
        {
            IList<EventModel> diffEvents = new List<EventModel>();
            string sqlQuery = @"
                SELECT
                    COALESCE(GoogleEvent.Id, DBEvent.Id) AS Id,
                    DBEvent.CalendarId,
                    DBEvent.ColorId,
                    DBEvent.CreatorId,
                    DBEvent.OrganizerId,
                    DBEvent.RecurringEventId,
                    DBEvent.ReminderId,
                    DBEvent.StartId,
                    DBEvent.EndId,
                    DBEvent.OriginalStartTimeId,
                    DBEvent.Sequence,
                    DBEvent.AnyoneCanAddSelf,
                    DBEvent.AttendeesOmitted,
                    DBEvent.CreatedRaw,
                    DBEvent.EDescription,
                    DBEvent.EndTimeUnspecified,
                    DBEvent.ETag,
                    DBEvent.EventType,
                    DBEvent.GuestsCanInviteOthers,
                    DBEvent.GuestsCanModify,
                    DBEvent.GuestsCanSeeOtherGuests,
                    DBEvent.HangoutLink,
                    DBEvent.HtmlLink,
                    DBEvent.ICalUID,
                    DBEvent.Location,
                    DBEvent.Kind,
                    DBEvent.Locked,
                    DBEvent.PrivateCopy,
                    DBEvent.EStatus,
                    DBEvent.Summary,
                    DBEvent.Transparency,
                    DBEvent.UpdatedRaw,
                    DBEvent.Visibility,
                    DBEvent.GoogleCreated,
                    DBEvent.Deleted
                FROM
                    @ListEventCheck AS GoogleEvent -- Event from Google Calendar
                FULL OUTER JOIN
                    Event AS DBEvent -- Event from Database
                ON
                    GoogleEvent.Id = DBEvent.Id
                LEFT JOIN
	                EventDateTime StartEvent
                ON
	                DBEvent.StartId = StartEvent.Id
                WHERE
                    (TRY_CAST(StartEvent.DateTimeRaw AS DATETIMEOFFSET) > SYSDATETIMEOFFSET() OR StartEvent.DateTimeRaw IS NULL) -- Upcomming event only
                    AND (DBEvent.CalendarId = @CalendarId OR DBEvent.CalendarId IS NULL) -- Specific CalendarId OR Add new from Google
                    AND (GoogleEvent.UpdatedRaw <> DBEvent.UpdatedRaw OR GoogleEvent.UpdatedRaw IS NULL OR DBEvent.UpdatedRaw IS NULL)
                    AND NOT (DBEvent.Deleted = 1 AND GoogleEvent.Id IS NULL); -- Deleted record both side";

            DataTable eventCheckTable = new DataTable();
            eventCheckTable.Columns.Add("Id", typeof(string));
            eventCheckTable.Columns.Add("UpdatedRaw", typeof(string));
            foreach (var ev in events)
            {
                eventCheckTable.Rows.Add(ev.Id, ev.UpdatedRaw);
            }

            EventModel eventModel;
            using (SqlCommand cmd = new SqlCommand(sqlQuery, Connection))
            {
                // Add the TVP as a parameter
                SqlParameter tvpParam = cmd.Parameters.AddWithValue("@ListEventCheck", eventCheckTable);
                tvpParam.SqlDbType = SqlDbType.Structured;
                tvpParam.TypeName = "UT_EventCheck";
                cmd.Parameters.AddWithValue("@CalendarId", calendarId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        eventModel = new EventModel()
                        {
                            Id = reader["Id"] as string,
                            CalendarId = reader["CalendarId"] as string,
                            AnyoneCanAddSelf = reader["AnyoneCanAddSelf"] as bool?,
                            AttendeesOmitted = reader["AttendeesOmitted"] as bool?,
                            ColorId = reader["ColorId"] as string,
                            CreatorId = reader["CreatorId"] as string,
                            OrganizerId = reader["OrganizerId"] as string,
                            ReminderId = reader["ReminderId"] as int?,
                            StartId = reader["StartId"] as int?,
                            EndId = reader["EndId"] as int?,
                            OriginalStartTimeId = reader["OriginalStartTimeId"] as int?,
                            CreatedRaw = reader["CreatedRaw"] as string,
                            Description = reader["EDescription"] as string,
                            EndTimeUnspecified = reader["EndTimeUnspecified"] as bool?,
                            ETag = reader["ETag"] as string,
                            EventType = reader["EventType"] as string,
                            GuestsCanInviteOthers = reader["GuestsCanInviteOthers"] as bool?,
                            GuestsCanModify = reader["GuestsCanModify"] as bool?,
                            GuestsCanSeeOtherGuests = reader["GuestsCanSeeOtherGuests"] as bool?,
                            HangoutLink = reader["HangoutLink"] as string,
                            HtmlLink = reader["HtmlLink"] as string,
                            ICalUID = reader["ICalUID"] as string,
                            Kind = reader["Kind"] as string,
                            Location = reader["Location"] as string,
                            Locked = reader["Locked"] as bool?,
                            PrivateCopy = reader["PrivateCopy"] as bool?,
                            RecurringEventId = reader["RecurringEventId"] as string,
                            Sequence = reader["Sequence"] as int?,
                            Status = reader["EStatus"] as string,
                            Summary = reader["Summary"] as string,
                            Transparency = reader["Transparency"] as string,
                            UpdatedRaw = reader["UpdatedRaw"] as string,
                            Visibility = reader["Visibility"] as string,
                            GoogleCreated = reader["GoogleCreated"] as bool?,
                            Deleted = reader["Deleted"] as bool?
                        };

                        EventDateTimeRepository eventDateTimeRepository = new EventDateTimeRepository(Connection);
                        if (eventModel.StartId != null)
                        {
                            eventModel.StartEventDateTime = eventDateTimeRepository.GetEventDateTimeById(eventModel.StartId);
                        }
                        if (eventModel.EndId != null)
                        {
                            eventModel.EndEventDateTime = eventDateTimeRepository.GetEventDateTimeById(eventModel.EndId);
                        }
                        if (eventModel.OriginalStartTimeId != null)
                        {
                            eventModel.OriginalStartTime = eventDateTimeRepository.GetEventDateTimeById(eventModel.OriginalStartTimeId);
                        }

                        // Creator
                        if (eventModel.CreatorId != null)
                        {
                            CreatorDataRepository creatorDataRepository = new CreatorDataRepository(Connection);
                            CreatorDataModel creatorDataModel = creatorDataRepository.SelectById(eventModel.CreatorId);
                            if (creatorDataModel != null)
                            {
                                eventModel.Creator = creatorDataModel;
                            }
                        }

                        // Organizer
                        if (eventModel.OrganizerId != null)
                        {
                            OrganizerDataRepository organizerDataRepository = new OrganizerDataRepository(Connection);
                            OrganizerDataModel organizerDataModel = organizerDataRepository.SelectById(eventModel.OrganizerId);
                            if (organizerDataModel != null)
                            {
                                eventModel.Organizer = organizerDataModel;
                            }
                        }

                        // Reminders
                        if (eventModel.ReminderId != null)
                        {
                            RemindersDataRepository remindersDataRepository = new RemindersDataRepository(Connection);
                            RemindersDataModel remindersDataModel = remindersDataRepository.SelectById(eventModel.ReminderId);
                            if (remindersDataModel != null)
                            {
                                eventModel.Reminders = remindersDataModel;
                            }
                        }

                        // Recurrence
                        RecurrenceRepository recurrenceRepository = new RecurrenceRepository(Connection);
                        IList<string> recurrenceList = recurrenceRepository.GetListByEventId(eventModel.Id);
                        if (recurrenceList.Count > 0)
                        {
                            eventModel.Recurrence = recurrenceList;
                        }

                        // Attendee
                        EventAttendeeRepository eventAttendeeRepository = new EventAttendeeRepository(Connection);
                        List<EventAttendeeModel> attendeeModelList = eventAttendeeRepository.SelectByEventId(eventModel.Id);
                        if (attendeeModelList.Count > 0)
                        {
                            eventModel.AttendeesModel = attendeeModelList;
                        }

                        diffEvents.Add(eventModel);
                    }
                }
            }
            return diffEvents;
        }
    }
}
