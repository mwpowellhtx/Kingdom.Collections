using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using Xunit;
    using static BitConverter;
    using static ImmutableBitArrayFixture;
    using static Elasticity;

    public class ImmutableBitArrayTests : TestFixtureBase
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private ImmutableBitArrayFixture Subject { get; set; }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                Subject = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Receives a new <paramref name="value"/> and assigns it to <see cref="Subject"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private ImmutableBitArrayFixture GetSubject(ImmutableBitArrayFixture value)
        {
            Assert.NotNull(Subject = value);
            return Subject;
        }

        [Theory, CombinatorialData]
        public void Verify_That_Length_Ctor_Correct([LengthValues] int length)
        {
            VerifyLengthBasedCtor(length);
        }

        [Theory, CombinatorialData]
        public void Verify_That_Length_Value_Ctor_Correct([LengthValues] int length, [ValueValues] bool value)
        {
            VerifyLengthBasedCtor(length, value);
        }

        private void VerifyLengthBasedCtor(int expectedLength, bool expectedValue = false)
        {
            var s = GetSubject(new ImmutableBitArrayFixture(expectedLength, expectedValue));
            Assert.Equal(expectedLength, s.Count);
            Assert.Equal(expectedLength, s.Length);
            Assert.Equal(expectedLength, s.Values.Count);
            Assert.True(s.Values.All(x => x == expectedValue));
        }

        [Theory, CombinatorialData]
        public void Verify_That_Bool_Values_Ctor_Correct([BoolValuesValues] BoolValuesFixture fixture)
        {
            VerifyValuesBasedCtor(fixture, f => new ImmutableBitArrayFixture(f.Values), s => s);
        }

        [Theory, CombinatorialData]
        public void Verify_That_Byte_Values_Ctor_Correct([ByteValuesValues] ByteValuesFixture fixture)
        {
            // Ditto in LSB order.
            VerifyValuesBasedCtor(fixture, f => new ImmutableBitArrayFixture(f.Values), s => s.ToBytes(false));
        }

        [Theory, CombinatorialData]
        public void Verify_That_UInt32_Values_Ctor_Correct([UInt32ValuesValues] UInt32ValuesFixture fixture)
        {
            VerifyValuesBasedCtor(fixture, f => new ImmutableBitArrayFixture(f.Values), s => s.ToInts(false));
        }

        private void VerifyValuesBasedCtor<TFixture, TValue>(TFixture fixture,
            Func<TFixture, ImmutableBitArrayFixture> getFixturedArray,
            Func<ImmutableBitArrayFixture, IEnumerable<TValue>> getValues)
            where TFixture : ValuesFixtureBase<TValue>
        {
            var s = GetSubject(getFixturedArray(fixture));
            var expectedLength = fixture.ExpectedLength;

            Assert.Equal(expectedLength, s.Count);
            Assert.Equal(expectedLength, s.Length);

            var expectedValues = fixture.Values.ToArray();
            var actualValues = getValues(s).ToArray();

            Assert.Equal(expectedValues, actualValues);
        }

        [Theory, CombinatorialData]
        public void Verify_That_Copy_Ctor_Correct([RandomIntValues] uint value)
        {
            var bytes = GetBytes(value);
            VerifyThatCopyCtorCorrect(() => FromBytes(bytes)
                , a => new ImmutableBitArrayFixture(a));
        }

        [Theory, CombinatorialData]
        public void Verify_That_Clone_Correct([RandomIntValues] uint value)
        {
            var bytes = GetBytes(value);
            VerifyThatCopyCtorCorrect(() => FromBytes(bytes)
                , a => a.Clone() as ImmutableBitArrayFixture);
        }

        /// <summary>
        /// Defines the UnaryOperation on the <paramref name="operand"/>
        /// of the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operand"></param>
        /// <returns></returns>
        private delegate T UnaryOperation<T>(T operand);

        /// <summary>
        /// Defines the BinaryOperand on <paramref name="lhs"/> and <paramref name="rhs"/>
        /// of the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        private delegate T BinaryOperation<T>(T lhs, T rhs);

        /// <summary>
        /// Defines a delegate for purposes of Verifying <paramref name="before"/> and
        /// <paramref name="after"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="before"></param>
        /// <param name="after"></param>
        private delegate void VerifyBeforeAndAfter<in T>(T before, T after);

        private void VerifyThatCopyCtorCorrect(Func<ImmutableBitArrayFixture> factory
            , UnaryOperation<ImmutableBitArrayFixture> copyOperation)
        {
            var s = GetSubject(factory());
            var copied = copyOperation(s);
            Assert.NotSame(copied, s);
            Assert.NotSame(copied.Values, s.Values);
            Assert.Equal(s, copied);
            Assert.Equal(s.Values, copied.Values);
        }

        [Theory, CombinatorialData]
        public void Verify_That_SetAll_Correct([LengthValues] int length, [ValueValues] bool value)
        {
            VerifyThatSetAllCorrect(new ImmutableBitArrayFixture(length, value), value, !value);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void VerifyThatSetAllCorrect(IImmutableBitArray fixture, bool sourceValue
            , bool destinationValue)
        {
            Assert.True(fixture.All(x => x == sourceValue));
            fixture.SetAll(destinationValue);
            Assert.True(fixture.All(x => x == destinationValue));

        }

        [Theory, CombinatorialData]
        public void Verify_That_SetGet_Correct([LengthValues] int length)
        {
            VerifyThatSetGetCorrect(new ImmutableBitArrayFixture(length), length
                , (arr, i) => arr.Get(i), (arr, i, value) => arr.Set(i, value));
        }

        [Theory, CombinatorialData]
        public void Verify_That_Indexer_Correct([LengthValues] int length)
        {
            VerifyThatSetGetCorrect(new ImmutableBitArrayFixture(length), length
                , (arr, i) => arr[i], (arr, i, value) => arr[i] = value);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void VerifyThatSetGetCorrect(IImmutableBitArray fixture, int length,
            Func<IImmutableBitArray, int, bool> getValue, Action<IImmutableBitArray, int, bool> setValue)
        {
            Assert.NotNull(fixture);
            Assert.Equal(length, fixture.Count);
            Assert.Equal(length, fixture.Length);

            for (var i = 0; i < fixture.Count; i++)
            {
                var value = getValue(fixture, i);
                Assert.False(value);

                setValue(fixture, i, true);

                value = getValue(fixture, i);
                Assert.True(value);
            }
        }

        [Theory, CombinatorialData]
        public void Verify_That_BinaryOperation_And_Correct(
            [RandomIntValues] uint aValue, [RandomIntValues] uint bValue)
        {
            VerifyThatBinaryOperationCorrect(aValue, bValue
                , (s, t) => s.InternalAnd(t), (x, y) => x & y);
        }

        [Theory, CombinatorialData]
        public void Verify_That_BinaryOperation_Or_Correct(
            [RandomIntValues] uint aValue, [RandomIntValues] uint bValue)
        {
            VerifyThatBinaryOperationCorrect(aValue, bValue
                , (s, t) => s.InternalOr(t), (x, y) => x | y);
        }

        [Theory, CombinatorialData]
        public void Verify_That_BinaryOperation_Xor_Correct([RandomIntValues] uint aValue
            , [RandomIntValues] uint bValue)
        {
            VerifyThatBinaryOperationCorrect(aValue, bValue
                , (s, t) => s.InternalXor(t), (x, y) => x ^ y);
        }

        private static void VerifyThatBinaryOperationCorrect(uint aValue, uint bValue
            , BinaryOperation<ImmutableBitArrayFixture> binaryOperation
            , BinaryOperation<uint> checker)
        {
            var a = FromBytes(GetBytes(aValue));
            var b = FromBytes(GetBytes(bValue));
            var c = binaryOperation(a, b);

            Assert.NotSame(a, c);
            Assert.NotSame(b, c);
            Assert.NotSame(a.Values, c.Values);
            Assert.NotSame(b.Values, c.Values);
            
            var cCheckValue = checker(aValue, bValue);
            var cCheckValues = GetBytes(cCheckValue);

            /* Asserting that the collections are equivalent is incorret, they must be equal.
             * That is, they must be the same size and order of the elements. */
            Assert.Equal(cCheckValues, c.ToBytes(false));
        }

        [Theory, CombinatorialData]
        public void Verify_That_UnaryOperation_Not_Correct([RandomIntValues] uint value)
        {
            /* This is much better factoring of the Verification that keeps
             the Ones Complement testing in the same place. */
            VerifyThatUnaryOperationCorrect(value, s => s.InternalNot(), (a, b) =>
            {
                Assert.NotNull(a);
                Assert.NotNull(b);

                var valuesBeforeOp = a.Select(x => x).ToArray();
                var valuesAfterOp = a.Select(x => x).ToArray();

                // TODO: TBD: this isn't really a general use verification.
                // Do not miss this since it is in large part the primary reason we are doing any of this.
                Assert.All(valuesBeforeOp.Zip(valuesAfterOp, (x, y) => x == y), Assert.True);
                Assert.All(valuesBeforeOp.Zip(b.Select(x => x).ToArray(), (x, y) => x != y), Assert.True);

                // Besides these we can do some additional more conventional assertions as we would expect.
                Assert.NotSame(a, b);
                Assert.NotSame(a.Values, b.Values);

                // Ditto before, must be the same count and values in the same order.
                Assert.Equal(GetBytes(~value), b.ToBytes(false));
            });
        }

        private static void VerifyThatUnaryOperationCorrect(uint value
            , UnaryOperation<ImmutableBitArrayFixture> unaryOperation
            , VerifyBeforeAndAfter<ImmutableBitArrayFixture> verify = null)
        {
            /* On second thought, I'm not positive this is quite "idempotence". However, it is
             pretty close. We do, however, expect that the original operand be left untouched
             by the Not or rather ~ (Ones Complement) operator. */

            var a = FromBytes(GetBytes(value));
            var b = unaryOperation(a);

            verify?.Invoke(a, b);
        }

        /* TODO: TBD: not going to worry about pulling the "arbitrarily long count" shift left/right
         forward. If it becomes necessary at some later point, then I will reconsider it, but for now,
         this is about as concise a revamping of the issue as could be determined. */
        private static int CalculateExpectedLengthAfterShift(uint value, int shift, Elasticity elasticity)
        {
            const int size = sizeof(uint) * 8;

            /* Determine which Bit Positions are set in the Value. As it turns out, all the Elasticity
             hoops we jumped through before are neatly captured by these calculations. */
            var bits = (from i in Enumerable.Range(0, size)
                where (value & (1 << i)) != 0
                select i + shift).ToArray();

            // TODO: TBD: arguably, "shift zero" should probably be its own corner case...
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (elasticity)
            {
                case Expansion:
                    // Nothing Expands, per se, in this use case.
                    return size + Math.Max(0, shift);
                case Contraction:
                    var selected = from x in bits where x < size && x >= 0 select x;
                    // ReSharper disable once PossibleMultipleEnumeration
                    return selected.Any()
                        // ReSharper disable once PossibleMultipleEnumeration
                        ? selected.Max() + 1
                        : 0;
                case Both:
                    return bits.Any(x => x >= 0) ? bits.Max() + 1 : 0;
                case None:
                    return size;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Verifies that <see cref="IImmutableBitArray{T}.ShiftLeft"/> behavior is correct.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <param name="elasticity"></param>
        [Theory, CombinatorialData]
        public void Verify_That_ShiftLeft_Correct([SpecificIntValues] uint value
            , [ShiftCountValues] int? count, [ElasticityValues] Elasticity elasticity)
        {
            var s = GetSubject(new ImmutableBitArrayFixture(GetBytes(value)));

            var expectedLength = CalculateExpectedLengthAfterShift(value, count ?? 1, elasticity);

            var shifted = count.HasValue
                ? s.InternalShiftLeft(count.Value, elasticity)
                : s.InternalShiftLeft(elasticity: elasticity);

            Assert.Equal(expectedLength, shifted.Length);
            Assert.Equal(expectedLength, shifted.Count);

            // Admittedly, this is a fairly narrow corner case.
            if (shifted.Any() && elasticity == Contraction)
            {
                Assert.True(shifted.Last());
            }
        }

        /// <summary>
        /// Verifies that <see cref="IImmutableBitArray{T}.ShiftRight"/> behavior is correct.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <param name="elasticity"></param>
        [Theory, CombinatorialData]
        public void Verify_That_ShiftRight_Correct([SpecificIntValues] uint value
            , [ShiftCountValues] int? count, [ElasticityValues] Elasticity elasticity)
        {
            var s = GetSubject(new ImmutableBitArrayFixture(GetBytes(value)));

            var expectedLength = CalculateExpectedLengthAfterShift(value, -(count ?? 1), elasticity);

            var shifted = count.HasValue
                ? s.InternalShiftRight(count.Value, elasticity)
                : s.InternalShiftRight(elasticity: elasticity);

            Assert.Equal(expectedLength, shifted.Length);
            Assert.Equal(expectedLength, shifted.Count);

            // Admittedly, this is a fairly narrow corner case.
            if (shifted.Any() && elasticity == Contraction)
            {
                Assert.True(shifted.Last());
            }
        }
    }
}
