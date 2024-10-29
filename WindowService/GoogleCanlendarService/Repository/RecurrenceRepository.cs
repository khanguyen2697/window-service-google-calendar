using System.Collections.Generic;
using System.Data.SqlClient;

namespace GoogleCanlendarService.Repository
{
    internal class RecurrenceRepository : BaseRepository
    {

        public RecurrenceRepository(SqlConnection connection, SqlTransaction transaction = null)
    : base(connection, transaction) { }

        public void InsertList(IList<string> recurrenceList, string eventId)
        {
            string StrQuery = @"
                INSERT INTO
                    Recurrence (EventId, Content) 
                VALUES
                    (@EventId, @Content)";
            using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection))
            {
                foreach (var recurrence in recurrenceList)
                {
                    cmd.Parameters.AddWithValue("@EventId", eventId);
                    cmd.Parameters.AddWithValue("@Content", recurrence);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IList<string> GetListByEventId(string eventId)
        {
            IList<string> recurrenceList = new List<string>();
            string strQuery = "SELECT Content FROM Recurrence WHERE EventId = @EventId";

            using (SqlCommand cmd = new SqlCommand(strQuery, this.Connection))
            {
                cmd.Parameters.AddWithValue("@EventId", eventId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string recurrence = reader["Content"].ToString();
                        recurrenceList.Add(recurrence);
                    }
                }
            }

            return recurrenceList;
        }
    }
}
