using System;

namespace Kingdom.Collections
{
    using Xunit;
    using Xunit.Abstractions;
    using static BitConverter;

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
    }
}
