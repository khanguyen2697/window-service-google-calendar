using System.Data.SqlClient;

namespace GoogleCanlendarService.Repository
{
    public class BaseRepository
    {
        protected SqlConnection Connection;
        protected SqlTransaction Transaction;

        public BaseRepository(SqlConnection connection, SqlTransaction transaction = null)
        {
            Connection = connection;
            Transaction = transaction;
        }
    }
}
