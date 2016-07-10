using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    internal abstract class ConnectionOrientedFixture : Disposable
    {
        protected static string GetConnectionString(string databaseName = "master")
        {
            var args = new[]
            {
                new KeyValuePair<string, string>("Data Source", "localhost"),
                new KeyValuePair<string, string>("Initial Catalog", databaseName),
                new KeyValuePair<string, string>("Integrated Security", "SSPI")
            };

            const string equals = "=";
            const string semiColon = ";";

            var connectionString = string.Join(semiColon, args.Select(
                arg => string.Join(equals, arg.Key, arg.Value)));

            return connectionString;
        }

        private static void VerifySqlConnectionOpen(SqlConnection conn)
        {
            conn.Open();

            Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
            Assert.That(conn.ClientConnectionId, Is.Not.EqualTo(Guid.Empty));
        }

        /// <summary>
        /// Returns a new <see cref="SqlConnection"/> given <paramref name="connectionString"/>.
        /// Passes that connection through the <paramref name="verify"/> action.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="verify">Passes the connection through the action. Default is
        /// <see cref="VerifySqlConnectionOpen"/>.</param>
        /// <returns></returns>
        private static SqlConnection GetSqlConnection(string connectionString,
            Action<SqlConnection> verify = null)
        {
            var conn = new SqlConnection(connectionString);

            Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            Assert.That(conn.ClientConnectionId, Is.EqualTo(Guid.Empty));

            verify = verify ?? VerifySqlConnectionOpen;

            verify(conn);

            return conn;
        }

        protected void Run(string connectionString, Action<SqlConnection> action = null)
        {
            action = action ?? (conn => { });

            using (var conn = GetSqlConnection(connectionString))
            {
                try
                {
                    action(conn);
                }
                finally
                {
                    // http://stackoverflow.com/questions/1145892/how-to-force-a-sqlconnection-to-physically-close-while-using-connection-pooling
                    SqlConnection.ClearPool(conn);
                }
            }
        }

        protected static void RunNonQuery(SqlConnection conn, string sql,
            params SqlParameter[] values)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddRange(values);
                cmd.ExecuteNonQuery();
            }
        }

        protected static IEnumerable<T> RunQuery<T>(SqlConnection conn, string sql,
            Func<SqlDataReader, T> getter, params SqlParameter[] values)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddRange(values);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return getter(reader);
                    }
                }
            }
        }
    }
}
