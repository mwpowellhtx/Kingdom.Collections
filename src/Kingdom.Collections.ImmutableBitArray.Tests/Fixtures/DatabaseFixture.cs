using System;

namespace Kingdom.Collections
{
    internal class DatabaseFixture : ConnectionOrientedFixture
    {
        public FixtureTableFixture FixtureTable { get; private set; }

        private string DatabaseName { get; set; }

        private Guid DatabaseId
        {
            get { return Guid.Parse(DatabaseName); }
            set { DatabaseName = value.ToString("D"); }
        }

        internal DatabaseFixture(Guid? did = null)
        {
            DatabaseId = did ?? Guid.NewGuid();

            VerifyDatabaseCreated(DatabaseName);

            FixtureTable = new FixtureTableFixture(DatabaseName);
        }

        private void VerifyDatabaseCreated(string databaseName)
        {
            Run(GetConnectionString(), conn =>
            {
                RunNonQuery(conn, string.Format(
                    @"IF DB_ID('{0}') IS NULL
CREATE DATABASE [{0}]", databaseName));
            });
        }

        private void VerifyDatabaseDropped(string databaseName)
        {
            Run(GetConnectionString(), conn =>
            {
                RunNonQuery(conn, string.Format(
                    @"IF DB_ID('{0}') IS NOT NULL
DROP DATABASE [{0}]", databaseName));
            });
        }

        protected override void Dispose(bool disposing)
        {
            VerifyDatabaseDropped(DatabaseName);

            base.Dispose(disposing);
        }
    }
}
