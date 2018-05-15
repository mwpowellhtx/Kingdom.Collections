using System;
using System.Linq;

namespace Kingdom.Collections
{
    public class UInt32ValuesFixture : ValuesFixtureBase<uint>
    {
        protected override string ToString(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            return string.Join(string.Empty, bytes.Select(b => $"{b:X2}")).ToLower();
        }

        public UInt32ValuesFixture()
            : base(() => sizeof(uint)*8)
        {
        }
    }
}
