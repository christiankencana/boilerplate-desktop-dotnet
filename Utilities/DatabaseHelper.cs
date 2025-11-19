using System;
using System.Data.SqlClient;
using System.Configuration;

namespace boilerplate_desktop_dotnet.Utilities
{
    public static class DatabaseHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString 
            ?? "Server=localhost;Database=boilerplate_desktop_dotnet;Integrated Security=true;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}