using System;
using System.Data.SqlClient;

namespace DatabaseApp;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            // Build our Connection String
            var connString = "Server=localhost;User Id=sa;Password=YOUR_PASSWORD;Initial Catalog=master";
            var builder = new SqlConnectionStringBuilder(connString);

            // Connect to the Docker SQL Server
            Console.Write("Connecting to SQL Server ... ");
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                Console.WriteLine("Done.");

                // Create the CustomerDB database
                Console.Write("Dropping and creating database 'CustomerDB' ... ");
                var createDbsql =
                    "DROP DATABASE IF EXISTS [CustomerDB]; CREATE DATABASE [CustomerDB]; USE [CustomerDB]";
                using (var command = new SqlCommand(createDbsql, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Done.");
                }

                // Create the Customers database table
                Console.Write("Creating the database table 'Customers' ... ");
                var createCustTableSql =
                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Customers')
                                  BEGIN
                                    CREATE TABLE [dbo].[Customers] (
                                       [CustomerID] INT            IDENTITY (1, 1) NOT NULL,
                                       [FirstName]  NVARCHAR (MAX) NULL,
                                       [LastName]   NVARCHAR (MAX) NULL,
                                       [Email]      NVARCHAR (MAX) NULL,
                                       CONSTRAINT [PK_dbo.Customers] PRIMARY KEY CLUSTERED ([CustomerID] ASC)
                                    )
                                  END";

                using (var command = new SqlCommand(createCustTableSql, connection))
                {
                    var affectedRows = command.ExecuteNonQuery();
                    Console.WriteLine("Done.");
                }

                // Insert user into the Customers table
                Console.Write("Inserting user into Customers table within the CustomerDB Database ...");
                var insertCustSql =
                    "INSERT INTO Customers (FirstName, LastName, Email) Values ('Basil', 'Brush', 'basil.brush@gmail.com');";
                using (var command = new SqlCommand(insertCustSql, connection))
                {
                    var affectedRows = command.ExecuteNonQuery();
                    Console.WriteLine("Done.");
                }

                // Retrieve all records that are within the Customers table
                var customerSql = "SELECT * From Customers";
                using (var command = new SqlCommand(customerSql, connection))
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"{reader.GetValue(0)}, {reader.GetValue(1)}, {reader.GetValue(2)}, {reader.GetValue(3)}");
                        reader.NextResult();
                    }

                    // Make sure to always close readers and connections.
                    reader.Close();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
