using System.Collections.Generic;

namespace Kingdom.Collections
{
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
            return new ImmutableBitArrayFixture(And(other));
        }

        internal ImmutableBitArrayFixture InternalOr(ImmutableBitArrayFixture other)
        {
            return new ImmutableBitArrayFixture(Or(other));
        }

        internal ImmutableBitArrayFixture InternalXor(ImmutableBitArrayFixture other)
        {
            return new ImmutableBitArrayFixture(Xor(other));
        }

        internal ImmutableBitArrayFixture InternalNot()
        {
            return new ImmutableBitArrayFixture(Not());
        }

        public override object Clone()
        {
            return new ImmutableBitArrayFixture(this);
        }
    }
}
