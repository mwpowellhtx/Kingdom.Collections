namespace Kingdom.Collections
{
    public class ByteValuesFixture : ValuesFixtureBase<byte>
    {
        protected override string ToString(byte value) => $"{value:X2}";

        public ByteValuesFixture()
            : base(() => sizeof(byte)*8)
        {
        }
    }
}
