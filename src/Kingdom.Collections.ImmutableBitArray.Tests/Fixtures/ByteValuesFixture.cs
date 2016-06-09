namespace Kingdom.Collections
{
    public class ByteValuesFixture : ValuesFixtureBase<byte>
    {
        protected override string ToString(byte value)
        {
            return string.Format("{0:X2}", value);
        }

        public ByteValuesFixture()
            : base(() => sizeof(byte)*8)
        {
        }
    }
}
