using System.Data.SqlClient;
using TestUpdateDatabase;

public class DBUtils
{

    private SqlConnection _connection;

    public DBUtils()
    {
        _connection = GetConnection();
        _connection.Open();
    }

    public int UpdateEventDateTime(int? id, string time)
    {
        int rowsAffected;
        string strQuery = @"
                UPDATE 
                    EventDateTime
                SET
                    DateTimeRaw = @DateTimeRaw
                WHERE
                    Id = @Id";

        using (SqlCommand cmd = new SqlCommand(strQuery, _connection))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@DateTimeRaw", time);

            rowsAffected = cmd.ExecuteNonQuery();
        }
        return rowsAffected;
    }

    public int UpdateEvent(Event eventObj)
    {
        int rowsAffected;
        string strQuery = @"
                UPDATE 
                    Event
                SET
                    Summary = @Summary,
                    UpdatedRaw = @UpdatedRaw
                WHERE
                    Id = @Id";

        using (SqlCommand cmd = new SqlCommand(strQuery, _connection))
        {
            cmd.Parameters.AddWithValue("@Id", eventObj.Id);
            cmd.Parameters.AddWithValue("@Summary", eventObj.Summary);
            cmd.Parameters.AddWithValue("@UpdatedRaw", GetCurrentUtcTimeString());

            rowsAffected = cmd.ExecuteNonQuery();
        }
        return rowsAffected;
    }

    public bool DeleteById(string id)
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
        using (SqlCommand cmd = new SqlCommand(strQuery, _connection))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UpdatedRaw", GetCurrentUtcTimeString());
            int rowsAffected = cmd.ExecuteNonQuery();
            isDeleted = rowsAffected > 0;
        }
        return isDeleted;
    }

    public string InsertCreator(Creator creator)
    {
        string StrQuery =
            @"INSERT INTO
                CreatorData (Id, Email, IsCreator)
            OUTPUT INSERTED.Id
            VALUES
                (@Id, @Email, 1)";
        using (SqlCommand cmd = new SqlCommand(StrQuery, _connection))
        {
            cmd.Parameters.AddWithValue("@Id", creator.Id);
            cmd.Parameters.AddWithValue("@Email", creator.Email);
            return cmd.ExecuteScalar().ToString();
        }
    }

    public int InsertEventDateTime(string dateTimeRaw)
    {
        int insertedId;
        string strQuery = @"
                INSERT INTO 
                    EventDateTime (Date, DateTimeRaw, TimeZone, ETag) OUTPUT INSERTED.Id
                VALUES
                    (@Date, @DateTimeRaw, @TimeZone, @ETag)";
        using (SqlCommand cmd = new SqlCommand(strQuery, _connection))
        {
            cmd.Parameters.AddWithValue("@Date", DBNull.Value);
            cmd.Parameters.AddWithValue("@DateTimeRaw", dateTimeRaw);
            cmd.Parameters.AddWithValue("@TimeZone", "Asia/Ho_Chi_Minh");
            cmd.Parameters.AddWithValue("@ETag", DBNull.Value);

            return (int)cmd.ExecuteScalar();
        }
    }

    public string InsertOrganizer(OrganizerData organizerData)
    {
        string StrQuery =
            @"INSERT INTO
                    OrganizerData (Id, DisplayName, Email, IsOrganizer) OUTPUT INSERTED.Id
                VALUES
                    (@Id, @DisplayName, @Email, @IsOrganizer)";
        using (SqlCommand cmd = new SqlCommand(StrQuery, _connection))
        {
            cmd.Parameters.AddWithValue("@Id", organizerData.Id);
            cmd.Parameters.AddWithValue("@DisplayName", DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", organizerData.Email);
            cmd.Parameters.AddWithValue("@IsOrganizer", 1);

            return cmd.ExecuteScalar().ToString();
        }
    }

    public string InsertEvent(Event eventObj)
    {
        string StrQuery =
                @"INSERT INTO Event (
                    Id,
                    CalendarId,
                    CreatorId,
                    OrganizerId,
                    StartId,
                    EndId,
                    Summary,
                    GoogleCreated
                ) 
                OUTPUT INSERTED.Id
                VALUES (
                    @Id,
                    @CalendarId,
                    @CreatorId,
                    @OrganizerId,
                    @StartId,
                    @EndId,
                    @Summary,
                    0
                )";
        using (SqlCommand cmd = new SqlCommand(StrQuery, _connection))
        {
            cmd.Parameters.AddWithValue("@Id", eventObj.Id);
            cmd.Parameters.AddWithValue("@CalendarId", eventObj.CalendarId);
            cmd.Parameters.AddWithValue("@CreatorId", eventObj.CreatorId);
            cmd.Parameters.AddWithValue("@OrganizerId", eventObj.OrganizerId);
            cmd.Parameters.AddWithValue("@StartId", eventObj.StartId);
            cmd.Parameters.AddWithValue("@EndId", eventObj.EndId);
            cmd.Parameters.AddWithValue("@Summary", eventObj.Summary);

            return cmd.ExecuteScalar().ToString();
        }
    }

    SqlConnection GetConnection()
    {
        //TODO update this to config file
        string _connectionString = "Server=localhost\\SQLExpress;Database=GoogleCalendar;User Id=khanta;Password=123456;";
        var connection = new SqlConnection(_connectionString);
        return connection;
    }

    public static string GetCurrentUtcTimeString()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}
