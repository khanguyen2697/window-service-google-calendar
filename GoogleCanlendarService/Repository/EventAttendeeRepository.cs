using GoogleCanlendarService.Models;
using GoogleCanlendarService.Repository;
using GoogleCanlendarService.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GoogleCanlendarService.Data
{
    class EventAttendeeRepository : BaseRepository
    {
        public EventAttendeeRepository(SqlConnection connection, SqlTransaction transaction = null)
            : base(connection, transaction) { }

        public void Insert(EventAttendeeModel eventAttendee)
        {
            string StrQuery =
                @"INSERT INTO EventAttendee (
                    Id,
                    EventId,
					DisplayName,
					Email,
					AdditionalGuests,
					Comment,
					Optional,
					Organizer,
					Resource,
					ResponseStatus,
					RepresentsCalendar,
					ETag
                ) VALUES (
				    @Id,
				    @EventId,
				    @DisplayName,
				    @Email,
				    @AdditionalGuests,
				    @Comment,
				    @Optional,
				    @Organizer,
				    @Resource,
				    @ResponseStatus,
				    @RepresentsCalendar,
				    @ETag
                )";
            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(StrQuery, this.Connection)
                : new SqlCommand(StrQuery, this.Connection, this.Transaction))
            {
                cmd.Parameters.AddWithValue("@Id", eventAttendee.Id);
                cmd.Parameters.AddWithValue("@EventId", eventAttendee.EventId);
                cmd.Parameters.AddWithValue("@DisplayName", DBUtils.GetDBValue(eventAttendee.DisplayName));
                cmd.Parameters.AddWithValue("@Email", DBUtils.GetDBValue(eventAttendee.Email));
                cmd.Parameters.AddWithValue("@AdditionalGuests", eventAttendee.AdditionalGuests ?? 0);
                cmd.Parameters.AddWithValue("@Comment", DBUtils.GetDBValue(eventAttendee.Comment));
                cmd.Parameters.AddWithValue("@Optional", eventAttendee.Optional ?? false);
                cmd.Parameters.AddWithValue("@Organizer", eventAttendee.Organizer ?? false);
                cmd.Parameters.AddWithValue("@Resource", eventAttendee.Resource ?? false);
                cmd.Parameters.AddWithValue("@ResponseStatus", DBUtils.GetDBValue(eventAttendee.ResponseStatus));
                cmd.Parameters.AddWithValue("@RepresentsCalendar", eventAttendee.Self ?? false);
                cmd.Parameters.AddWithValue("@ETag", DBUtils.GetDBValue(eventAttendee.ETag));

                cmd.ExecuteNonQuery();
            }
        }

        public List<EventAttendeeModel> SelectByEventId(string eventId)
        {
            string StrQuery = @"
                        SELECT
                            Id, EventId, DisplayName, Email, AdditionalGuests, Comment, Optional, Organizer, Resource, ResponseStatus, RepresentsCalendar, ETag
                        FROM
                            EventAttendee
                        WHERE
                            EventId = @EventId";
            List<EventAttendeeModel> eventAttendees = new List<EventAttendeeModel>();

            using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection))
            {
                cmd.Parameters.AddWithValue("@EventId", eventId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EventAttendeeModel eventAttendee = new EventAttendeeModel
                        {
                            Id = reader["Id"].ToString(),
                            EventId = reader["EventId"].ToString(),
                            DisplayName = reader["DisplayName"] != DBNull.Value ? reader["DisplayName"].ToString() : null,
                            Email = reader["Email"].ToString(),
                            AdditionalGuests = reader["AdditionalGuests"] != DBNull.Value ? (int)reader["AdditionalGuests"] : (int?)null,
                            Comment = reader["Comment"] != DBNull.Value ? reader["Comment"].ToString() : null,
                            Optional = reader["Optional"] != DBNull.Value ? (bool)reader["Optional"] : (bool?)null,
                            Organizer = reader["Organizer"] != DBNull.Value ? (bool)reader["Organizer"] : (bool?)null,
                            Resource = reader["Resource"] != DBNull.Value ? (bool)reader["Resource"] : (bool?)null,
                            ResponseStatus = reader["ResponseStatus"] != DBNull.Value ? reader["ResponseStatus"].ToString() : null,
                            ETag = reader["ETag"] != DBNull.Value ? reader["ETag"].ToString() : null,
                            Self = reader["RepresentsCalendar"] != DBNull.Value ? (bool)reader["RepresentsCalendar"] : (bool?)null
                        };

                        eventAttendees.Add(eventAttendee);
                    }
                }
            }

            return eventAttendees;
        }
    }
}
