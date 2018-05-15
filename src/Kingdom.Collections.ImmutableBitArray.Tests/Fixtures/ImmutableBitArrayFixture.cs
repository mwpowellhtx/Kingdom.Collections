using System.Collections.Generic;

namespace Kingdom.Collections
{
    using Xunit;

    /// <summary>
    /// Helps by exposing <see cref="ImmutableBitArray._values"/> into the unit tests
    /// for verification.
    /// </summary>
    /// <inheritdoc cref="ImmutableBitArray"/>
    public class ImmutableBitArrayFixture : ImmutableBitArray
    {
        internal List<bool> Values => _values;

       public ImmutableBitArrayFixture(ImmutableBitArray other)
            : base(other)
        {
        }

        public ImmutableBitArrayFixture(IEnumerable<bool> values)
            : base(values)
        {
        }

        /// <summary>
        /// Construct the fixture with <paramref name="bytes"/> in LSB.
        /// </summary>
        /// <param name="bytes"></param>
        /// <inheritdoc />
        public ImmutableBitArrayFixture(IEnumerable<byte> bytes)
            : base(bytes)
        {
        }

        /// <summary>
        /// Construct the fixture with <paramref name="values"/> in LSB.
        /// </summary>
        /// <param name="values"></param>
        /// <inheritdoc />
        public ImmutableBitArrayFixture(IEnumerable<uint> values)
            : base(values)
        {
        }

        public ImmutableBitArrayFixture(int length)
            : base(length)
        {
        }

        public ImmutableBitArrayFixture(int length, bool defaultValue)
            : base(length, defaultValue)
        {
        }

        public static ImmutableBitArrayFixture FromBytes(params byte[] bytes)
        {
            return new ImmutableBitArrayFixture(bytes);
        }

        internal ImmutableBitArrayFixture InternalAnd(ImmutableBitArrayFixture other)
        {
            var result = And(other);
            Assert.NotNull(result);
            return new ImmutableBitArrayFixture(result);
        }

        internal ImmutableBitArrayFixture InternalOr(ImmutableBitArrayFixture other)
        {
            var result = Or(other);
            Assert.NotNull(result);
            return new ImmutableBitArrayFixture(result);
        }

        internal ImmutableBitArrayFixture InternalXor(ImmutableBitArrayFixture other)
        {
            var result = Xor(other);
            Assert.NotNull(result);
            return new ImmutableBitArrayFixture(result);
        }

        internal ImmutableBitArrayFixture InternalNot()
        {
            var result = Not();
            Assert.NotNull(result);
            return new ImmutableBitArrayFixture(result);
        }

        internal ImmutableBitArrayFixture InternalShiftLeft(int count = 1, Elasticity elasticity = Elasticity.None)
        {
            var result = ShiftLeft(count, elasticity);
            Assert.NotNull(result);
            Assert.NotSame(this, result);
            return new ImmutableBitArrayFixture(result);
        }

        internal ImmutableBitArrayFixture InternalShiftRight(int count = 1, Elasticity elasticity = Elasticity.None)
        {
            var result = ShiftRight(count, elasticity);
            Assert.NotNull(result);
            Assert.NotSame(this, result);
            return new ImmutableBitArrayFixture(result);
        }

        public override object Clone()
        {
            return new ImmutableBitArrayFixture(this);
        }
    }
}
