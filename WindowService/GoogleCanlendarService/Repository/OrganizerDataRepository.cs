using GoogleCanlendarService.Models;
using GoogleCanlendarService.Repository;
using GoogleCanlendarService.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GoogleCanlendarService.Data
{
    class OrganizerDataRepository : BaseRepository
    {
        public OrganizerDataRepository(SqlConnection connection, SqlTransaction transaction = null)
            : base(connection, transaction) { }

        public string Insert(OrganizerDataModel organizerData)
        {
            string insertedId = null;
            string StrQuery =
                @"INSERT INTO
                    OrganizerData (Id, DisplayName, Email, IsOrganizer)
                VALUES
                    (@Id, @DisplayName, @Email, @IsOrganizer)
                SELECT @Id";
            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(StrQuery, this.Connection)
                : new SqlCommand(StrQuery, this.Connection, this.Transaction))
            {
                cmd.Parameters.AddWithValue("@Id", organizerData.Id);
                cmd.Parameters.AddWithValue("@DisplayName", DBUtils.GetDBValue(organizerData.DisplayName));
                cmd.Parameters.AddWithValue("@Email", organizerData.Email);
                cmd.Parameters.AddWithValue("@IsOrganizer", organizerData.Self.HasValue ? (object)organizerData.Self.Value : false);

                insertedId = cmd.ExecuteScalar().ToString();
            }

            return insertedId;
        }

        public OrganizerDataModel SelectById(string id)
        {
            string StrQuery = "SELECT * FROM OrganizerData WHERE Id = @Id";
            OrganizerDataModel organizerData = null;

            using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        organizerData = new OrganizerDataModel
                        {
                            Id = reader["Id"].ToString(),
                            DisplayName = reader["DisplayName"] != DBNull.Value ? reader["DisplayName"].ToString() : null,
                            Email = reader["Email"].ToString(),
                            Self = reader["IsOrganizer"] != DBNull.Value ? (bool?)reader["IsOrganizer"] : null
                        };
                    }
                }
            }
            return organizerData;
        }

        public IList<OrganizerDataModel> SelectAll()
        {
            string StrQuery = "SELECT * FROM OrganizerData";
            IList<OrganizerDataModel> organizers = new List<OrganizerDataModel>();
            using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OrganizerDataModel organizerData = new OrganizerDataModel
                        {
                            Id = reader["Id"].ToString(),
                            DisplayName = reader["DisplayName"] != DBNull.Value ? reader["DisplayName"].ToString() : null,
                            Email = reader["Email"].ToString(),
                            Self = reader["IsOrganizer"] != DBNull.Value ? (bool?)reader["IsOrganizer"] : null
                        };
                        organizers.Add(organizerData);
                    }
                }
            }
            return organizers;
        }
    }
}
