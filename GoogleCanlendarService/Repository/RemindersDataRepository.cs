using GoogleCanlendarService.Models;
using GoogleCanlendarService.Repository;
using GoogleCanlendarService.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace GoogleCanlendarService.Repository
{
    class RemindersDataRepository : BaseRepository
    {
        public RemindersDataRepository(SqlConnection connection, SqlTransaction transaction = null)
            : base(connection, transaction) { }

        public int Insert(RemindersDataModel remindersData)
        {
            string StrQuery = @"
                INSERT INTO 
                    RemindersData (UseDefault) OUTPUT INSERTED.Id
                VALUES 
                    (@UseDefault);";

            int remindersDataId;
            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(StrQuery, this.Connection)
                : new SqlCommand(StrQuery, this.Connection, this.Transaction))
            {
                cmd.Parameters.AddWithValue("@UseDefault", DBUtils.GetDBValue(remindersData.UseDefault));

                // Execute the query and get the inserted RemindersData Id
                remindersDataId = (int)cmd.ExecuteScalar();
            }

            EventReminderRepository eventReminderRepository = new EventReminderRepository(this.Connection, this.Transaction);

            // Insert each EventReminder into EventReminder table
            foreach (var eventReminder in remindersData.OverridesModel)
            {
                eventReminder.RemindersDataId = remindersDataId;
                eventReminderRepository.Insert(eventReminder);
            }

            return remindersDataId;
        }

        public List<RemindersDataModel> SelectAll()
        {
            List<RemindersDataModel> remindersDataList = new List<RemindersDataModel>();

            string selectRemindersDataQuery = @"
                SELECT
                    rd.Id, rd.UseDefault, er.Method, er.Minutes, er.ETag
                FROM
                    RemindersData rd
                LEFT JOIN
                    EventReminder er
                ON rd.Id = er.RemindersDataId";

            using (SqlCommand cmd = new SqlCommand(selectRemindersDataQuery, this.Connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Get the RemindersData Id and UseDefault
                        int id = reader.GetInt32(0);
                        bool? useDefault = reader.IsDBNull(1) ? (bool?)null : reader.GetBoolean(1);

                        // Find or create the RemindersDataModel for this Id
                        RemindersDataModel remindersData = remindersDataList.FirstOrDefault(r => r.Id == id);
                        if (remindersData == null)
                        {
                            remindersData = new RemindersDataModel
                            {
                                Id = id,
                                UseDefault = useDefault,
                                OverridesModel = new List<EventReminderModel>()
                            };
                            remindersDataList.Add(remindersData);
                        }

                        // Create and add the EventReminderModel if it exists
                        if (!reader.IsDBNull(2)) // Check if Method is not NULL
                        {
                            var eventReminder = new EventReminderModel
                            {
                                Method = reader.GetString(2),
                                Minutes = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                ETag = reader.IsDBNull(4) ? null : reader.GetString(4)
                            };

                            remindersData.Overrides.Add(eventReminder);
                        }
                    }
                }
            }

            return remindersDataList;
        }
    }
}
