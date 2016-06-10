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
            VerifyValuesBasedCtor(fixture, f => new ImmutableBitArrayFixture(f.Values.ToArray()), s => s.ToBytes());
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

            CollectionAssert.AreEquivalent(cCheckValues, c.ToBytes());
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

            CollectionAssert.AreEquivalent(bCheckValues, b.ToBytes());
        }
    }
}
