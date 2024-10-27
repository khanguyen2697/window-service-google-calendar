using System.Configuration;
using System.Data.SqlClient;

namespace GoogleCanlendarService.Utils
{
    public class DBConnection
    {
        public static SqlConnection GetConnection()
        {
            string server = ConfigurationManager.AppSettings["DbServer"];
            string database = ConfigurationManager.AppSettings["DbName"];
            string user = ConfigurationManager.AppSettings["DbUser"];
            string password = ConfigurationManager.AppSettings["DbPassword"];
            string connectionString = $"Server={server};Database={database};User Id={user};Password={password};MultipleActiveResultSets=true;";
            var connection = new SqlConnection(connectionString);
            return connection;
        }
    }
}
