using System;
using System.Linq;

namespace Talks.Collections
{
    using Kingdom.Collections;
    using Xunit;
    using Xunit.Abstractions;
    using static BitConverter;

    // ReSharper disable once UnusedMember.Global
    public class KingdomCollectionsImmutableBitArrayTests : BitArrayComparisonTests<ImmutableBitArray>
    {
        // ReSharper disable once UnusedMember.Global
        public KingdomCollectionsImmutableBitArrayTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        protected override bool ShouldOperatorsMutateOperands { get; } = false;

        protected override ImmutableBitArray GetSubject(int value) => new ImmutableBitArray(GetBytes(value));

        protected override BinaryOperationCallback<ImmutableBitArray> AndCallback => (a, b) => a.And(b);

        protected override BinaryOperationCallback<ImmutableBitArray> OrCallback => (a, b) => a.Or(b);

        protected override BinaryOperationCallback<ImmutableBitArray> XorCallback => (a, b) => a.Xor(b);

        protected override UnaryOperationCallback<ImmutableBitArray> NotCallback => a => a.Not();

        protected override void VerifyBits(ImmutableBitArray subject, int value)
        {
            // We do not have to resort to a bit level comparison.
            var bytes = subject.ToBytes().ToArray();
            Assert.Equal(sizeof(int), bytes.Length);
            // Reverse the Converted Bytes for proper LSB order.
            Assert.Equal(GetBytes(value).Reverse(), bytes);
        }
    }
}
