using GoogleCanlendarService.Models;
using GoogleCanlendarService.Repository;
using GoogleCanlendarService.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GoogleCanlendarService.Data
{
    class CreatorDataRepository : BaseRepository
    {
        public CreatorDataRepository(SqlConnection connection, SqlTransaction transaction = null)
            : base(connection, transaction) { }

        public string Insert(CreatorDataModel creatorData)
        {
            string insertedId = null;
            string StrQuery =
                @"INSERT INTO
                    CreatorData (Id, DisplayName, Email, IsCreator)
                VALUES
                    (@Id, @DisplayName, @Email, @IsCreator)
                SELECT @Id";
            using (SqlCommand cmd = (this.Transaction == null)
                ? new SqlCommand(StrQuery, this.Connection)
                : new SqlCommand(StrQuery, this.Connection, this.Transaction))
            {
                cmd.Parameters.AddWithValue("@Id", creatorData.Id);
                cmd.Parameters.AddWithValue("@DisplayName", DBUtils.GetDBValue(creatorData.DisplayName));
                cmd.Parameters.AddWithValue("@Email", DBUtils.GetDBValue(creatorData.Email));
                cmd.Parameters.AddWithValue("@IsCreator", creatorData.Self.HasValue ? (object)creatorData.Self.Value : false);

                insertedId = cmd.ExecuteScalar().ToString();
            }

            return insertedId;
        }
        public CreatorDataModel SelectById(string id)
        {
            string StrQuery = "SELECT * FROM CreatorData WHERE Id = @Id";
            CreatorDataModel creatorData = null;

            using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        creatorData = new CreatorDataModel
                        {
                            Id = reader["Id"].ToString(),
                            DisplayName = reader["DisplayName"] != DBNull.Value ? reader["DisplayName"].ToString() : null,
                            Email = reader["Email"].ToString(),
                            Self = reader["IsCreator"] != DBNull.Value ? (bool?)reader["IsCreator"] : null
                        };
                    }
                }
            }
            return creatorData;
        }

        public IList<CreatorDataModel> SelectAll()
        {
            string StrQuery = "SELECT * FROM CreatorData";
            IList<CreatorDataModel> creators = new List<CreatorDataModel>();
            using (SqlCommand cmd = new SqlCommand(StrQuery, this.Connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CreatorDataModel creatorData = new CreatorDataModel
                        {
                            Id = reader["Id"].ToString(),
                            DisplayName = reader["DisplayName"] != DBNull.Value ? reader["DisplayName"].ToString() : null,
                            Email = reader["Email"].ToString(),
                            Self = reader["IsCreator"] != DBNull.Value ? (bool?)reader["IsCreator"] : null
                        };
                        creators.Add(creatorData);
                    }
                }
            }
            return creators;
        }
    }
}
