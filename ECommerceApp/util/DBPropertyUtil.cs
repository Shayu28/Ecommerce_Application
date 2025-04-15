using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ECommerceApp.main;



namespace ECommerceApp.util
{
    public static class DBPropertyUtil
    {
        public static string GetConnectionString(string propertyFileName)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, propertyFileName);
            Console.WriteLine($"DEBUG: Looking for properties at: {path}");

            try
            {
                string propertiesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, propertyFileName);
                Console.WriteLine($"DEBUG: Properties path: {path}");
                Console.WriteLine($"DEBUG: File exists: {File.Exists(path)}");

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Database properties file not found at: {path}");
                }

               
                var lines = File.ReadAllLines(path);
                var properties = new System.Collections.Generic.Dictionary<string, string>();
                Console.WriteLine("DEBUG: File contents:");
                foreach (var line in lines) Console.WriteLine(line);

                // ... rest of your parsing code ...

                string connStr = $"Server=SHAYU;Database=ECommerceDB;User Id=ecom_app;Password=ecom;TrustServerCertificate=True;";
                Console.WriteLine($"DEBUG: Generated connection string: {connStr}");
                return connStr;

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        properties[parts[0].Trim()] = parts[1].Trim();
                    }
                }

                // Build the connection string with additional parameters
                return $"Server={properties["SHAYU"]};" +
                       $"Database={properties["ECommerceDB"]};" +
                       $"User Id={properties["ecom_app"]};" +
                       $"Password={properties["ecom"]};" +
                       "TrustServerCertificate=True;" +  // Bypass certificate validation for development
                       "Connection Timeout=30;";          // 30-second connection timeout
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create connection string: " + ex.Message);
            }

        }
    }
}