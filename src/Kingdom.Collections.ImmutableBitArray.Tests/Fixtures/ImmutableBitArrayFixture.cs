using System.Collections.Generic;

namespace Kingdom.Collections
{
    using NUnit.Framework;
    /// <summary>
    /// Helps by exposing <see cref="ImmutableBitArray._values"/> into the unit tests
    /// for verification.
    /// </summary>
    internal class ImmutableBitArrayFixture : ImmutableBitArray
    {
        internal List<bool> Values
        {
            get { return _values; }
        }

       public ImmutableBitArrayFixture(ImmutableBitArray other)
            : base(other)
        {
        }

        public ImmutableBitArrayFixture(bool[] values)
            : base(values)
        {
        }

        public ImmutableBitArrayFixture(byte[] bytes)
            : base(bytes)
        {
        }

        public ImmutableBitArrayFixture(uint[] values)
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
            Assert.That(result, Is.Not.Null);
            return new ImmutableBitArrayFixture(result);
        }

        internal ImmutableBitArrayFixture InternalOr(ImmutableBitArrayFixture other)
        {
            var result = Or(other);
            Assert.That(result, Is.Not.Null);
            return new ImmutableBitArrayFixture(result);
        }

        internal ImmutableBitArrayFixture InternalXor(ImmutableBitArrayFixture other)
        {
            var result = Xor(other);
            Assert.That(result, Is.Not.Null);
            return new ImmutableBitArrayFixture(result);
        }

        internal ImmutableBitArrayFixture InternalNot()
        {
            var result = Not();
            Assert.That(result, Is.Not.Null);
            return new ImmutableBitArrayFixture(result);
        }

        internal ImmutableBitArrayFixture InternalShiftLeft(int count = 1, bool elastic = false)
        {
            var result = ShiftLeft(count, elastic);
            Assert.That(result, Is.Not.Null);
            return new ImmutableBitArrayFixture(result);
        }

        internal ImmutableBitArrayFixture InternalShiftRight(int count = 1, bool elastic = false)
        {
            var result = ShiftRight(count, elastic);
            Assert.That(result, Is.Not.Null);
            return new ImmutableBitArrayFixture(result);
        }

        public override object Clone()
        {
            return new ImmutableBitArrayFixture(this);
        }
    }
}
