using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceApp.util
{
    public static class DBConnUtil
    {
        public static SqlConnection GetConnection(string connectionString)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string connStr = "Server=SHAYU;Database=EcommerceDB;User Id=ecom_app;Password=ecom;";
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            Console.WriteLine("Connected successfully!");
            try
            {
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                throw;
            }
        }
    }
}