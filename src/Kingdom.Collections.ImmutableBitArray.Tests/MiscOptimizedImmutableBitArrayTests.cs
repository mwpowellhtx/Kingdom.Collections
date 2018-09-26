using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using Xunit;
    using Xunit.Abstractions;
    using static BitConverter;
    using static OptimizedImmutableBitArray;
    using static String;

    public partial class MiscOptimizedImmutableBitArrayTests : SubjectTestFixtureBase<OptimizedImmutableBitArray>
    {
        public MiscOptimizedImmutableBitArrayTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Theory, CombinatorialData]
        public void Verify_that_Clear_works_correctly([RandomIntValues] uint value)
        {
            GetSubject(() => CreateBitArray(value));
            Assert.Equal(sizeof(uint) * 8, Subject.Length);
            Subject.Clear();
            Assert.Empty(Subject.ToBytes());
            Assert.Equal(0, Subject.Length);
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
            /* This is a legit Assertion to perform considering the Subject.
             * Therefore, override whatever Xunit and/or R# may be complaining about. */
            Assert.Equal(0, Subject.Count);
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
        }

        [Theory, CombinatorialData]
        public void Verify_that_Clone_works_correctly([RandomIntValues] uint value)
        {
            GetSubject(() => CreateBitArray(value));
            OptimizedImmutableBitArray clone;
            Assert.NotNull(clone = Subject.Clone() as OptimizedImmutableBitArray);
            Assert.NotSame(Subject, clone);
            Assert.Equal(Subject.ToBytes(), clone.ToBytes());
        }

        [Theory, CombinatorialData]
        public void Verify_that_Equal_arrays_work_correctly([RandomIntValues] uint value)
        {
            var a = CreateBitArray(value);
            var b = CreateBitArray(value);
            Assert.NotSame(a, b);
            var expected = GetEndianAwareBytes(value);
            var isBigEndian = !IsLittleEndian;
            Assert.Equal(expected, a.ToBytes(isBigEndian));
            Assert.Equal(a.ToBytes(), b.ToBytes());
            Assert.True(a.Equals(b));
        }

        [Theory, CombinatorialData]
        public void Verify_that_Not_Equal_arrays_work_correctly([RandomIntValues] uint x, [RandomIntValues] uint y)
        {
            // TODO: TBD: run this through actual MemberData and ensure that X and Y are both different.
            // TODO: TBD: for now at this late hour this is the poor man's...
            if (x == y)
            {
                return;
            }
            var a = CreateBitArray(x);
            var b = CreateBitArray(y);
            Assert.NotSame(a, b);
            var expectedX = GetEndianAwareBytes(x);
            var expectedY = GetEndianAwareBytes(y);
            var isBigEndian = !IsLittleEndian;
            Assert.Equal(expectedX, a.ToBytes(isBigEndian));
            Assert.Equal(expectedY, b.ToBytes(isBigEndian));
            Assert.NotEqual(a.ToBytes(), b.ToBytes());
            Assert.False(a.Equals(b));
        }

        /// <summary>
        /// Verifies that the Length and Count respond properly to <paramref name="bytes"/>
        /// Initialization as well as to changes in
        /// <see cref="OptimizedImmutableBitArray.Length"/> via <paramref name="lengthDelta"/>.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="lengthDelta">Permits changes to
        /// <see cref="OptimizedImmutableBitArray.Length"/> allowing it to either expand or
        /// contract accordingly, which may also yield <see cref="ArgumentOutOfRangeException"/>
        /// when excessive negative delta values are provided.</param>
        /// <remarks>We could ostensibly separate this into several more focused individual unit
        /// tests, but we can just as easily leverage the differences in furnished
        /// <see cref="MemberDataAttribute"/>.</remarks>
        [Theory, MemberData(nameof(LengthCountData))]
        public void Length_and_Count_work_correctly(IEnumerable<byte> bytes, int lengthDelta)
        {
            Assert.NotNull(bytes);
            bytes = bytes.ToArray();
            GetSubject(() => CreateBitArrayWithArray(bytes.ToArray()));

            // Let's make some helpful reports during the unit test for post-mortem analysis.
            OutputHelper.WriteLine(
                "Original bytes were: "
                + $"{Join(", ", Subject.InternalBytes().Select(b => $"'{b:X2}'"))}"
            );

            var initialExpectedLengthAndCount = bytes.Count() * BitCount;
            Assert.Equal(initialExpectedLengthAndCount, Subject.Length);
            Assert.Equal(initialExpectedLengthAndCount, Subject.Count);

            var expectedLength = Subject.Length + lengthDelta;
            var expectedCount = (expectedLength % BitCount == 0
                                    ? expectedLength / BitCount
                                    : expectedLength / BitCount + 1) * BitCount;

            if (expectedLength < 0)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => Subject.Length += lengthDelta)
                    .WithExceptionDetail(aoorex =>
                    {
                        OutputHelper.WriteLine($"Expected {nameof(ArgumentOutOfRangeException)} was thrown: {aoorex}");
                        // Capture the value and leverage its nameof at the same time.
                        var value = expectedLength;
                        Assert.Equal(nameof(value), aoorex.ParamName);
                        Assert.Equal(value, aoorex.ActualValue);
                    });

                return;
            }

            // This is the money shot right here.
            Subject.Length += lengthDelta;
            Assert.Equal(expectedLength, Subject.Length);
            Assert.Equal(expectedCount, Subject.Count);

            // ReSharper disable once InvertIf do not invert this since we use bitIndex subsequently.
            // Last but not least verify that any Bits on the Boundary Byte exceeding Length are reset.
            if (Subject.Length % BitCount is int bitPosition && bitPosition != 0)
            {
                var boundaryIndex = Subject.Length / BitCount;
                var actualBoundaryByte = Subject.InternalBytes().ElementAt(boundaryIndex);

                OutputHelper.WriteLine(
                    $"New Length '{Subject.Length}' occurs within Byte Boundary '{boundaryIndex}'"
                    + $" at bit index '{bitPosition - 1}' with unmasked byte '{actualBoundaryByte:X2}'."
                );

                /* So this may not be so Kosher, but there are only so many ways
                 * to Makean appropriate Mask in the problem domain. */

                // BitPosition, not to be confused with the actual BitIndex.
                var boundaryByteMask = MakeMask(0, bitPosition - 1);
                const byte expectedMaskedByte = default(byte);
                Assert.Equal(expectedMaskedByte, actualBoundaryByte & (byte) ~boundaryByteMask);
            }
        }
    }
}
