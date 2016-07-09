using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    public class ImmutableBitArrayTests : TestFixtureBase
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private ImmutableBitArrayFixture Subject { get; set; }

        public override void TearDown()
        {
            Subject = null;

            base.TearDown();
        }

        [Test]
        public void Verify_That_Length_Ctor_Correct([LengthValues] int length)
        {
            VerifyLengthBasedCtor(length);
        }

        [Test]
        public void Verify_That_Length_Value_Ctor_Correct([LengthValues] int length, [ValueValues] bool value)
        {
            VerifyLengthBasedCtor(length, value);
        }

        private void VerifyLengthBasedCtor(int expectedLength, bool expectedValue = false)
        {
            var s = Subject = new ImmutableBitArrayFixture(expectedLength, expectedValue);
            Assert.That(s, Has.Count.EqualTo(expectedLength));
            Assert.That(s, Has.Length.EqualTo(expectedLength));
            Assert.That(s.Values, Has.Count.EqualTo(expectedLength));
            Assert.That(s.Values.All(x => x == expectedValue));
        }

        [Test]
        public void Verify_That_Bool_Values_Ctor_Correct([BoolValuesValues] BoolValuesFixture fixture)
        {
            VerifyValuesBasedCtor(fixture, f => new ImmutableBitArrayFixture(f.Values.ToArray()), s => s);
        }

        [Test]
        public void Verify_That_Byte_Values_Ctor_Correct([ByteValuesValues] ByteValuesFixture fixture)
        {
            // Ditto in LSB order.
            VerifyValuesBasedCtor(fixture, f => new ImmutableBitArrayFixture(f.Values.ToArray()), s => s.ToBytes(false));
        }

        [Test]
        public void Verify_That_UInt32_Values_Ctor_Correct([UInt32ValuesValues] UInt32ValuesFixture fixture)
        {
            VerifyValuesBasedCtor(fixture, f => new ImmutableBitArrayFixture(f.Values.ToArray()), s => s.ToInts());
        }

        private void VerifyValuesBasedCtor<TFixture, TValue>(TFixture fixture,
            Func<TFixture, ImmutableBitArrayFixture> getFixturedArray,
            Func<ImmutableBitArrayFixture, IEnumerable<TValue>> getValues)
            where TFixture : ValuesFixtureBase<TValue>
        {
            var s = Subject = getFixturedArray(fixture);
            var expectedLength = fixture.ExpectedLength;

            Assert.That(s, Has.Count.EqualTo(expectedLength));
            Assert.That(s, Has.Length.EqualTo(expectedLength));

            var expectedValues = fixture.Values;
            var actualValues = getValues(s);

            CollectionAssert.AreEquivalent(expectedValues, actualValues);
        }

        [Test]
        public void Verify_That_Copy_Ctor_Correct([RandomIntValues] uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            VerifyThatCopyCtorCorrect(() => ImmutableBitArrayFixture.FromBytes(bytes),
                a => new ImmutableBitArrayFixture(a));
        }

        [Test]
        public void Verify_That_Clone_Correct([RandomIntValues] uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            VerifyThatCopyCtorCorrect(() => ImmutableBitArrayFixture.FromBytes(bytes),
                a => a.Clone() as ImmutableBitArrayFixture);
        }

        private void VerifyThatCopyCtorCorrect(Func<ImmutableBitArrayFixture> factory,
            Func<ImmutableBitArrayFixture, ImmutableBitArrayFixture> copier)
        {
            var s = Subject = factory();
            var copied = copier(s);
            Assert.That(s, Is.Not.SameAs(copied));
            Assert.That(s.Values, Is.Not.SameAs(copied.Values));
            CollectionAssert.AreEquivalent(s, copied);
            CollectionAssert.AreEquivalent(s.Values, copied.Values);
        }

        [Test]
        public void Verify_That_SetAll_Correct([LengthValues] int length,
            [ValueValues] bool value)
        {
            VerifyThatSetAllCorrect(new ImmutableBitArrayFixture(length, value), value, !value);
        }

        private static void VerifyThatSetAllCorrect(IImmutableBitArray fixture,
            bool sourceValue, bool destinationValue)
        {
            var allSource = fixture.All(x => x == sourceValue);
            Assert.That(allSource);

            fixture.SetAll(destinationValue);

            var allDestination = fixture.All(x => x == destinationValue);
            Assert.That(allDestination);
        }

        [Test]
        public void Verify_That_SetGet_Correct([LengthValues] int length)
        {
            VerifyThatSetGetCorrect(new ImmutableBitArrayFixture(length), length,
                (arr, i) => arr.Get(i), (arr, i, value) => arr.Set(i, value));
        }

        [Test]
        public void Verify_That_Indexer_Correct([LengthValues] int length)
        {
            VerifyThatSetGetCorrect(new ImmutableBitArrayFixture(length), length,
                (arr, i) => arr[i], (arr, i, value) => arr[i] = value);
        }

        private static void VerifyThatSetGetCorrect(IImmutableBitArray fixture, int length,
            Func<IImmutableBitArray, int, bool> getValue, Action<IImmutableBitArray, int, bool> setValue)
        {
            Assert.That(fixture, Is.Not.Null);
            Assert.That(fixture, Has.Count.EqualTo(length));
            Assert.That(fixture, Has.Length.EqualTo(length));

            for (var i = 0; i < fixture.Count; i++)
            {
                var value = getValue(fixture, i);
                Assert.That(value, Is.False);

                setValue(fixture, i, true);

                value = getValue(fixture, i);
                Assert.That(value, Is.True);
            }
        }

        [Test]
        public void Verify_That_BinaryOperation_And_Correct(
            [RandomIntValues] uint aValue, [RandomIntValues] uint bValue)
        {
            VerifyThatBinaryOperationCorrect(aValue, bValue, (a, b) => a.InternalAnd(b), (a, b) => a & b);
        }

        [Test]
        public void Verify_That_BinaryOperation_Or_Correct(
            [RandomIntValues] uint aValue, [RandomIntValues] uint bValue)
        {
            VerifyThatBinaryOperationCorrect(aValue, bValue, (a, b) => a.InternalOr(b), (a, b) => a | b);
        }

        [Test]
        public void Verify_That_BinaryOperation_Xor_Correct(
            [RandomIntValues] uint aValue, [RandomIntValues] uint bValue)
        {
            VerifyThatBinaryOperationCorrect(aValue, bValue, (a, b) => a.InternalXor(b), (a, b) => a ^ b);
        }

        private static void VerifyThatBinaryOperationCorrect(uint aValue, uint bValue,
            Func<ImmutableBitArrayFixture, ImmutableBitArrayFixture, ImmutableBitArrayFixture> operation,
            Func<uint, uint, uint> checker)
        {
            var a = ImmutableBitArrayFixture.FromBytes(BitConverter.GetBytes(aValue));
            var b = ImmutableBitArrayFixture.FromBytes(BitConverter.GetBytes(bValue));
            var c = operation(a, b);

            Assert.That(a, Is.Not.SameAs(c));
            Assert.That(b, Is.Not.SameAs(c));
            Assert.That(a.Values, Is.Not.SameAs(c.Values));
            Assert.That(b.Values, Is.Not.SameAs(c.Values));
            
            var cCheckValue = checker(aValue, bValue);
            var cCheckValues = BitConverter.GetBytes(cCheckValue);

            /* Asserting that the collections are equivalent is incorret, they must be equal.
             * That is, they must be the same size and order of the elements. */
            CollectionAssert.AreEqual(cCheckValues, c.ToBytes(false));
        }

        [Test]
        public void Verify_That_BinaryOperation_Not_Correct(
            [RandomIntValues] uint value)
        {
            VerifyThatUnaryOperationCorrect(value, a => a.InternalNot(), a => ~a);
        }

        private static void VerifyThatUnaryOperationCorrect(uint value,
            Func<ImmutableBitArrayFixture, ImmutableBitArrayFixture> operation,
            Func<uint, uint> checker)
        {
            var a = ImmutableBitArrayFixture.FromBytes(BitConverter.GetBytes(value));
            var b = operation(a);

            Assert.That(a, Is.Not.SameAs(b));
            Assert.That(a.Values, Is.Not.SameAs(b.Values));

            var bCheckValue = checker(value);
            var bCheckValues = BitConverter.GetBytes(bCheckValue);

            // Ditto before, must be the same count and values in the same order.
            CollectionAssert.AreEqual(bCheckValues, b.ToBytes(false));
        }

        /// <summary>
        /// Verifies the eighty percent use cases of the shift left operation.
        /// Leaving corner cases for more targeted test methods.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <param name="elastic"></param>
        [Test]
        public void Verify_That_SkipLeft_Correct([SpecificIntValues] uint value,
            [ShiftCountValues] int? count, [ElasticValues] bool elastic)
        {
            var actualCount = count ?? 1;
            // This is a constraint of the language itself, not of the bit array.
            Assert.That(actualCount, Is.LessThanOrEqualTo(31));
            var bytes = BitConverter.GetBytes(value);
            VerifyShiftOperationCorrect(new ImmutableBitArrayFixture(bytes),
                arr => arr.InternalShiftLeft(actualCount, elastic),
                value << actualCount, actualCount, elastic);
        }

        /// <summary>
        /// Verifies the eighty percent use cases of the shift left operation.
        /// Leaving corner cases for more targeted test methods.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <param name="elastic"></param>
        [Test]
        public void Verify_That_ShiftRight_Correct([SpecificIntValues] uint value,
            [ShiftCountValues] int? count, [ElasticValues] bool elastic)
        {
            var actualCount = count ?? 1;
            // This is a constraint of the language itself, not of the bit array.
            Assert.That(actualCount, Is.LessThanOrEqualTo(31));
            var bytes = BitConverter.GetBytes(value);
            VerifyShiftOperationCorrect(new ImmutableBitArrayFixture(bytes),
                arr => arr.InternalShiftRight(actualCount, elastic),
                value >> actualCount, -actualCount, elastic);
        }

        private static void VerifyShiftOperationCorrect(ImmutableBitArrayFixture fixture,
            Func<ImmutableBitArrayFixture, ImmutableBitArrayFixture> shift,
            uint expectedValue, int count, bool elastic)
        {
            Assert.That(fixture, Is.Not.Null);

            const int size = sizeof(uint)*8;

            Assert.That(fixture, Has.Count.EqualTo(size));
            Assert.That(fixture, Has.Length.EqualTo(size));

            var shifted = shift(fixture);
            var expectedCount = elastic ? Math.Max(size + count, 0) : size;

            Assert.That(shifted, Is.Not.SameAs(fixture));
            Assert.That(shifted, Has.Count.EqualTo(expectedCount));
            Assert.That(shifted, Has.Length.EqualTo(expectedCount));

            var shiftedValues = shifted.ToInts(false);

            // Does not matter, per se, what the second element is, but the first element should be this.
            Assert.That(shiftedValues.ElementAt(0), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase((uint) 0x12341234, 96, true)]
        [TestCase((uint) 0x23452345, 128, false)]
        [TestCase((uint) 0x34563456, 192, true)]
        [TestCase((uint) 0x45674567, 256, false)]
        public void Verify_That_ShiftLeft_ArbitrarilyLongCount(uint value, int count, bool elastic)
        {
            // Virtually the same in approach saving for a couple of left/right, positive/negative arguments.
            VerifyThatShiftOperationArbitrarilyLongCountCorrect(
                new ImmutableBitArrayFixture(BitConverter.GetBytes(value)),
                arr => arr.InternalShiftLeft(count, elastic), count, elastic);
        }

        [Test]
        [TestCase((uint) 0x12341234, 96, true)]
        [TestCase((uint) 0x23452345, 128, false)]
        [TestCase((uint) 0x34563456, 192, true)]
        [TestCase((uint) 0x45674567, 256, false)]
        public void Verify_That_ShiftRight_ArbitrarilyLongCount(uint value, int count, bool elastic)
        {
            // Virtually the same in approach saving for a couple of left/right, positive/negative arguments.
            VerifyThatShiftOperationArbitrarilyLongCountCorrect(
                new ImmutableBitArrayFixture(BitConverter.GetBytes(value)),
                arr => arr.InternalShiftRight(count, elastic), -count, elastic);
        }

        private static void VerifyThatShiftOperationArbitrarilyLongCountCorrect(ImmutableBitArrayFixture fixture, 
            Func<ImmutableBitArrayFixture, ImmutableBitArrayFixture> shift, int count,  bool elastic)
        {
            Assert.That(fixture, Is.Not.Null);

            var shifted = shift(fixture);
            Assert.That(shifted, Is.Not.SameAs(fixture));

            var expectedCount = elastic ? Math.Max(fixture.Length + count, 0) : fixture.Length;

            Assert.That(shifted, Has.Count.EqualTo(expectedCount));
            Assert.That(shifted, Has.Length.EqualTo(expectedCount));
        }
    }
}
