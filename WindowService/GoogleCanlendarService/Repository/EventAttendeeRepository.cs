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
					Email,
					DisplayName,
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
				    @Email,
				    @DisplayName,
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
                cmd.Parameters.AddWithValue("@Id", DBUtils.GetDBValue(eventAttendee.Id));
                cmd.Parameters.AddWithValue("@EventId", eventAttendee.EventId);
                cmd.Parameters.AddWithValue("@Email", DBUtils.GetDBValue(eventAttendee.Email));
                cmd.Parameters.AddWithValue("@DisplayName", DBUtils.GetDBValue(eventAttendee.DisplayName));
                cmd.Parameters.AddWithValue("@AdditionalGuests", DBUtils.GetDBValue(eventAttendee.AdditionalGuests));
                cmd.Parameters.AddWithValue("@Comment", DBUtils.GetDBValue(eventAttendee.Comment));
                cmd.Parameters.AddWithValue("@Optional", DBUtils.GetDBValue(eventAttendee.Optional));
                cmd.Parameters.AddWithValue("@Organizer", DBUtils.GetDBValue(eventAttendee.Organizer));
                cmd.Parameters.AddWithValue("@Resource", DBUtils.GetDBValue(eventAttendee.Resource));
                cmd.Parameters.AddWithValue("@ResponseStatus", DBUtils.GetDBValue(eventAttendee.ResponseStatus));
                cmd.Parameters.AddWithValue("@RepresentsCalendar", DBUtils.GetDBValue(eventAttendee.Self));
                cmd.Parameters.AddWithValue("@ETag", DBUtils.GetDBValue(eventAttendee.ETag));

                cmd.ExecuteNonQuery();
            }
        }

        public List<EventAttendeeModel> SelectByEventId(string eventId)
        {
            string StrQuery = @"
                        SELECT
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

        public int DeleteByEventId(string eventId)
        {
            string StrQuery = @"
                DELETE FROM
                    EventAttendee
                WHERE
                    EventId = @EventId";

            using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection))
            {
                cmd.Parameters.AddWithValue("@EventId", eventId);

                // Execute the delete command
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected;
            }
        }
    }
}
