using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Insights
{
    public class DatabaseConnections
    {
        internal static IEnumerable<string> connectionStringsToIgnore = null;

        /// <summary>
        /// Prevents a default instance of the <see cref="DatabaseConnections"/> class from being created.
        /// </summary>
        static DatabaseConnections()
        {
            DatabaseConnections.connectionStringsToIgnore = (ConfigurationManager.AppSettings["devops.insights:ConnectionStringsToIgnore"] ?? string.Empty).Split(';').ToList();
        }

        /// <summary>
        /// Retrieves the database connection information dependencies
        /// </summary>
        /// <param name="connectionStrings">The connection strings of the databases to ping.</param>
        /// 
        public static IEnumerable<DatabaseConnectionInformation> RetrieveDatabaseConnectionInformation(ConnectionStringSettingsCollection connectionStrings)
        {

            ConcurrentBag<DatabaseConnectionInformation> databaseConnectionsInformation = new ConcurrentBag<DatabaseConnectionInformation>();

            foreach (ConnectionStringSettings connectionStringSetting in connectionStrings)
            {
                if (DatabaseConnections.connectionStringsToIgnore.Contains(connectionStringSetting.Name))
                {
                    continue;
                }

                try
                {
                    SqlConnection connection = new SqlConnection(connectionStringSetting.ConnectionString);

                    // Machine configurations pretty much always have this in there, this will enver be used in any environments so ignore it.
                    if (connection.DataSource == ".\\SQLEXPRESS")
                    {
                        continue;
                    }

                    DatabaseConnectionInformation connectionInformation = new DatabaseConnectionInformation();

                    try
                    {
                        connectionInformation.ServerName = connection.DataSource;
                        connectionInformation.ConnectionStringName = connectionStringSetting.Name;
                        connectionInformation.IsAlive = true;
                        SqlCommand command = new SqlCommand("SELECT @@Version", connection);

                        connection.Open();
                        connectionInformation.Version = (string)command.ExecuteScalar();
                    }
                    catch (Exception e)
                    {
                        connectionInformation.IsAlive = false;
                        connectionInformation.StatusMessage = e.Message;
                    }

                    databaseConnectionsInformation.Add(connectionInformation);

                    connection.Dispose();
                }
                catch
                {
                    databaseConnectionsInformation.Add(new DatabaseConnectionInformation() { IsAlive = false, StatusMessage = string.Format("Invalid connection string for '{0}': ", connectionStringSetting.Name) });
                }
            };

            return databaseConnectionsInformation.Select(db => db);
        }
    }
}
