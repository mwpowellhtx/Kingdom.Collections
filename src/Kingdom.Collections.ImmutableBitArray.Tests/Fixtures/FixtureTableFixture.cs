using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    /// <summary>
    /// This class represents the real rub, just besides the infrastructure work on the database
    /// being created. We want to verify that saving a byte array, be it from whichever source,
    /// is saved and restored in a certain order. This class does no verification on the validity
    /// of the data, other than that the data itself saved and was gotten in the given order.
    /// </summary>
    internal class FixtureTableFixture : ConnectionOrientedFixture
    {
        private readonly string _tableName;

        private readonly string _databaseName;

        internal FixtureTableFixture(string databaseName)
        {
            _tableName = "fixture";

            _databaseName = databaseName;

            CreateFixtureTable();
        }

        private void CreateFixtureTable()
        {
            const string tableName = "fixture";

            Run(GetConnectionString(_databaseName), conn =>
            {
                RunNonQuery(conn, string.Format(
                    @"IF OBJECT_ID('{0}', 'U') IS NULL
CREATE TABLE [dbo].[{0}] (
    [Id] [UNIQUEIDENTIFIER]
        CONSTRAINT [PK_{0}] PRIMARY KEY
        CONSTRAINT [DF_{0}_Id] DEFAULT NEWSEQUENTIALID(),
    [Bytes] [VARBINARY](MAX) NULL
        CONSTRAINT [DF_{0}_Bytes] DEFAULT NULL,
    [CreatedOn] [DATETIME] NOT NULL
        CONSTRAINT [DF_{0}_CreatedOn] DEFAULT GETUTCDATE()
)", tableName));
            });
        }

        public class Record
        {
            internal Guid Id { get; set; }

            internal byte[] Bytes { get; set; }

            internal DateTime CreatedOn { get; set; }
        }

        /// <summary>
        /// Inserts the <paramref name="bytes"/> into the table and reports back the
        /// <see cref="Record"/> corresponding to what was just inserted. This represents an
        /// entirely different instance in memory, in order so that adequate verification can take
        /// place.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public Record InsertBytes(IEnumerable<byte> bytes)
        {
            Record record = null;

            Run(GetConnectionString(_databaseName), conn =>
            {
                var args = new[]
                {
                    new SqlParameter("bytes", SqlDbType.VarBinary) {Value = bytes.ToArray()}
                };

                record = RunQuery(conn,
                    string.Format(
                        @"DECLARE @results AS TABLE(Id UNIQUEIDENTIFIER, Bytes VARBINARY(MAX), CreatedOn DATETIME);
INSERT INTO {0} ([Bytes]) OUTPUT inserted.* INTO @results VALUES (@bytes);
SELECT * FROM @results;", _tableName),
                    reader => new Record
                    {
                        Id = (Guid) reader["Id"],
                        Bytes = (byte[]) reader["Bytes"],
                        CreatedOn = (DateTime) reader["CreatedOn"]
                    },
                    args).Single();
            });

            // Comparing CreatedOn does not really prove anything for what we want here.
            Assert.That(record, Is.Not.Null);
            Assert.That(record.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(record.Bytes, Is.Not.SameAs(bytes));
            CollectionAssert.AreEqual(record.Bytes, bytes);

            return record;
        }
    }
}
