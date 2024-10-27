using GoogleCanlendarService.Models;
using GoogleCanlendarService.Repository;
using GoogleCanlendarService.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GoogleCanlendarService.Data
{
    class EventAttachmentRepository : BaseRepository
    {
        public EventAttachmentRepository(SqlConnection connection, SqlTransaction transaction = null)
            : base(connection, transaction) { }

        public void Insert(EventAttachmentModel eventAttachment)
        {
            string StrQuery =
                @"INSERT INTO
                    EventAttachment (FileId, EventId, FileUrl, IconLink, MimeType, Title, ETag)
                VALUES
                    (@FileId, @EventId, @FileUrl, @IconLink, @MimeType, @Title, @ETag)";

            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(StrQuery, this.Connection)
                : new SqlCommand(StrQuery, this.Connection, this.Transaction))
            {
                cmd.Parameters.AddWithValue("@FileId", eventAttachment.FileId);
                cmd.Parameters.AddWithValue("@EventId", eventAttachment.EventId);
                cmd.Parameters.AddWithValue("@FileUrl", DBUtils.GetDBValue(eventAttachment.FileUrl));
                cmd.Parameters.AddWithValue("@IconLink", DBUtils.GetDBValue(eventAttachment.IconLink));
                cmd.Parameters.AddWithValue("@MimeType", DBUtils.GetDBValue(eventAttachment.MimeType));
                cmd.Parameters.AddWithValue("@Title", DBUtils.GetDBValue(eventAttachment.Title));
                cmd.Parameters.AddWithValue("@ETag", DBUtils.GetDBValue(eventAttachment.ETag));

                cmd.ExecuteNonQuery();
            }
        }

        public List<EventAttachmentModel> SelectByEventId(string eventId)
        {
            string StrQuery = "SELECT * FROM EventAttachment WHERE EventId = @EventId";
            List<EventAttachmentModel> eventAttachments = new List<EventAttachmentModel>();

            using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection))
            {
                cmd.Parameters.AddWithValue("@EventId", eventId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EventAttachmentModel attachment = new EventAttachmentModel
                        {
                            FileId = reader["FileId"].ToString(),
                            EventId = reader["EventId"].ToString(),
                            FileUrl = reader["FileUrl"].ToString(),
                            IconLink = reader["IconLink"].ToString(),
                            MimeType = reader["MimeType"].ToString(),
                            Title = reader["Title"].ToString(),
                            ETag = reader["ETag"].ToString()
                        };
                        eventAttachments.Add(attachment);
                    }
                }
            }
            return eventAttachments;
        }
    }
}
