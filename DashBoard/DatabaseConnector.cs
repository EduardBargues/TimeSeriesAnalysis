using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security;
using MoreLinq;

namespace DashBoard
{
    internal class DatabaseConnector : IDisposable
    {
        private SqlConnection connection;
        private SqlTransaction transaction;
        private SecureString securePassword;

        public void Connect(string connectionString, string userName, string password)
        {
            securePassword = new SecureString();
            foreach (char c in password)
                securePassword.AppendChar(c);
            securePassword.MakeReadOnly();
            SqlCredential credentials = new SqlCredential(userName, securePassword);
            connection = new SqlConnection(connectionString, credentials);
            connection.Open();
        }
        public IEnumerable<(string, object)[]> ExecuteReaderCommand(string commandText)
        {
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand(commandText, connection, transaction);
            using (command)
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    (string, object)[] foo = new(string, object)[reader.FieldCount];
                    for (int index = 0; index < reader.FieldCount; index++)
                        foo[index] = (reader.GetName(index), reader[index]);
                    yield return foo;
                }
                reader.Close();
            }
        }
        public void Disconnect()
        {
            transaction?.Commit();
            connection?.Close();
        }
        public void Dispose()
        {
            transaction?.Dispose();
            connection?.Dispose();
            securePassword?.Dispose();
        }
    }
}