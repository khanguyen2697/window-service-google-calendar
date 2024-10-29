using GoogleCanlendarService.Models;
using GoogleCanlendarService.Utils;
using System.Data.SqlClient;

namespace GoogleCanlendarService.Repository
{
    class EventReminderRepository : BaseRepository
    {
        public EventReminderRepository(SqlConnection connection, SqlTransaction transaction = null)
            : base(connection, transaction) { }

        public void Insert(EventReminderModel eventReminder)
        {
            string StrQuery = @"
                INSERT INTO 
                    EventReminder (Method, Minutes, ETag, RemindersDataId) 
                VALUES
                    (@Method, @Minutes, @ETag, @RemindersDataId)";
            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(StrQuery, this.Connection)
                : new SqlCommand(StrQuery, this.Connection, this.Transaction))
            {
                cmd.Parameters.AddWithValue("@RemindersDataId", eventReminder.RemindersDataId);
                cmd.Parameters.AddWithValue("@Method", DBUtils.GetDBValue(eventReminder.Method));
                cmd.Parameters.AddWithValue("@Minutes", DBUtils.GetDBValue(eventReminder.Minutes));
                cmd.Parameters.AddWithValue("@ETag", DBUtils.GetDBValue(eventReminder.ETag));

                cmd.ExecuteNonQuery();
            }
        }
    }
}
