using GoogleCanlendarService.Models;
using GoogleCanlendarService.Utils;
using System.Data.SqlClient;

namespace GoogleCanlendarService.Repository
{
    class EventDateTimeRepository : BaseRepository
    {
        public EventDateTimeRepository(SqlConnection connection, SqlTransaction transaction = null)
            : base(connection, transaction) { }

        public int Insert(EventDateTimeModel eventDateTimeModel)
        {
            int insertedId;
            string strQuery = @"
                INSERT INTO 
                    EventDateTime (Date, DateTimeRaw, TimeZone, ETag) OUTPUT INSERTED.Id
                VALUES
                    (@Date, @DateTimeRaw, @TimeZone, @ETag)";
            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(strQuery, this.Connection)
                : new SqlCommand(strQuery, this.Connection, this.Transaction))
            {
                cmd.Parameters.AddWithValue("@Date", DBUtils.GetDBValue(eventDateTimeModel.Date));
                cmd.Parameters.AddWithValue("@DateTimeRaw", DBUtils.GetDBValue(eventDateTimeModel.DateTimeRaw));
                cmd.Parameters.AddWithValue("@TimeZone", DBUtils.GetDBValue(eventDateTimeModel.TimeZone));
                cmd.Parameters.AddWithValue("@ETag", DBUtils.GetDBValue(eventDateTimeModel.ETag));

                insertedId = (int)cmd.ExecuteScalar();
            }

            return insertedId;
        }

        public int Update(EventDateTimeModel eventDateTimeModel)
        {
            int rowsAffected;
            string strQuery = @"
                UPDATE 
                    EventDateTime
                SET
                    Date = @Date,
                    DateTimeRaw = @DateTimeRaw,
                    TimeZone = @TimeZone,
                    ETag = @ETag
                WHERE
                    Id = @Id";

            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(strQuery, this.Connection)
                : new SqlCommand(strQuery, this.Connection, this.Transaction))
            {
                cmd.Parameters.AddWithValue("@Date", DBUtils.GetDBValue(eventDateTimeModel.Date));
                cmd.Parameters.AddWithValue("@DateTimeRaw", DBUtils.GetDBValue(eventDateTimeModel.DateTimeRaw));
                cmd.Parameters.AddWithValue("@TimeZone", DBUtils.GetDBValue(eventDateTimeModel.TimeZone));
                cmd.Parameters.AddWithValue("@ETag", DBUtils.GetDBValue(eventDateTimeModel.ETag));
                cmd.Parameters.AddWithValue("@Id", eventDateTimeModel.Id);

                rowsAffected = cmd.ExecuteNonQuery();
            }

            return rowsAffected;
        }

        public EventDateTimeModel GetEventDateTimeById(int? Id)
        {
            string StrQuery = @"
                SELECT
                    Id, 
                    Date,
                    DateTimeRaw,
                    TimeZone,
                    ETag
                FROM
                    EventDateTime
                WHERE
                    Id = @Id";
            EventDateTimeModel eventDateTime = null;
            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(StrQuery, this.Connection)
                : new SqlCommand(StrQuery, this.Connection, this.Transaction))
            {
                cmd.Parameters.AddWithValue("@Id", Id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        eventDateTime = new EventDateTimeModel
                        {
                            Id = reader["Id"] as int?,
                            Date = reader["Date"] as string,
                            DateTimeRaw = reader["DateTimeRaw"] as string,
                            TimeZone = reader["TimeZone"] as string,
                            ETag = reader["ETag"] as string
                        };
                    }

                }
            }

            return eventDateTime;
        }
    }
}