using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlClient;
using CsvHelper;
using System.Globalization;

namespace ETL_RDS
{   
    class BabyNames
    {
        public string Name { get; set; }
        public string Gender { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string girlPath = "C:/Users/19498/Downloads/girlNames.csv";
            string boyPath = "C:/Users/19498/Downloads/boyNames.csv";

            string girl = "girl";
            string boy = "boy";

            string database = "BabyNames";
            string girlTable = "GirlNames";
            string boyTable = "BoyNames";
            string username = Environment.GetEnvironmentVariable("DB_USERNAME");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            string server = Environment.GetEnvironmentVariable("SERVER");

            string connect = $"Server={server};Database={database};User Id={username};Password={password};";

            ETL(connect, girlTable, girlPath, girl);
            ETL(connect, boyTable, boyPath, boy);

        }   

        static void ETL(string connectionString, string tableName, string filepath, string gender)
        {
            List<BabyNames> babycsv = ReadCsv(filepath, gender);
            CreateTable(connectionString, tableName);
            LoadData(babycsv, connectionString, tableName);   
        }

        static List<BabyNames> ReadCsv(string filepath, string gender)
        {
            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = new List<BabyNames>();

                while (csv.Read())
                {
                    var record = new BabyNames
                    {
                        Name = csv.GetField<string>(0),
                        Gender = gender
                    };

                    records.Add(record);              
                }
                return records;
            }
        }

        static void CreateTable(string connectionString, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{tableName}' AND xtype='U')
                    CREATE TABLE {tableName}(
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(155),
                        Gender NVARCHAR(10)
                    )";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        static void LoadData(List<BabyNames> data, string connectionString, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {   
                connection.Open();
                foreach (var row in data)
                {
                    string insertQuery = $@"
                        INSERT INTO {tableName} (Name, Gender)
                        VALUES (@Name, @Gender)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", row.Name);
                        command.Parameters.AddWithValue("@Gender", row.Gender);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}