using System;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    public class DatabaseSerializationTests : TestFixtureBase
    {
        private DatabaseFixture Db { get; set; }

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            Db = new DatabaseFixture();
        }

        public override void TestFixtureTearDown()
        {
            Db.Dispose();

            base.TestFixtureTearDown();
        }

        /// <summary>
        /// Verifies that bit array serialization is correct.
        /// </summary>
        /// <param name="mask"></param>
        [Test]
        public void VerifyThatBitArraySerializesCorrectly([SerializedMaskValues] uint mask)
        {
            var subject = CreateBitArray(mask);

            // Make sure that we report them in the correct, expected LSB order.
            var record = Db.FixtureTable.InsertBytes(subject.ToBytes(false));

            CreateBitArray(record.Bytes,
                s =>
                {
                    Assert.That(s.Length, Is.EqualTo(record.Bytes.Length*8));
                    Assert.That(s.Equals(subject));
                });
        }

        private static ImmutableBitArrayFixture VerifyBitArray(ImmutableBitArrayFixture subject,
            Action<ImmutableBitArrayFixture> verify = null)
        {
            verify = verify ?? (x => { });

            Assert.That(subject, Is.Not.Null);

            verify(subject);

            return subject;
        }

        private static ImmutableBitArrayFixture CreateBitArray(byte[] bytes,
            Action<ImmutableBitArrayFixture> verify)
        {
            return VerifyBitArray(new ImmutableBitArrayFixture(bytes), verify);
        }

        /// <summary>
        /// Returns a created fully vetted array based on <paramref name="mask"/>.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        private static ImmutableBitArrayFixture CreateBitArray(uint mask)
        {
            var result = VerifyBitArray(new ImmutableBitArrayFixture(new[] {mask}),
                s =>
                {
                    Assert.That(s.Length, Is.EqualTo(sizeof(uint)*8));

                    // Reverse these bytes because they are in reverse order.
                    var maskBytes = BitConverter.GetBytes(mask).ToArray();

                    var subjectBytes = s.ToBytes(false).ToArray();

                    CollectionAssert.AreEqual(subjectBytes, maskBytes);
                });

            return result;
        }
    }
}
