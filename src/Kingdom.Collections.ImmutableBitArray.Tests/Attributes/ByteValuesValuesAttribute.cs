using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    internal class ByteValuesValuesAttribute : ValuesValuesAttributeBase
    {
        private static IEnumerable<ByteValuesFixture> GetValuesFixtures(int length)
        {
            var values = Enumerable.Range(0, length).Select(_ => default(byte)).ToArray();
            if (length <= 2)
            {
                yield return new ByteValuesFixture {Values = values};
                yield break;
            }
            var middle = values.Length/2;
            const int maxShifts = sizeof(byte)*8;
            for (var shift = 0; shift < maxShifts; shift += 2)
            {
                // Yes, we will merge the fields thus accumulating a byte in the mix.
                values[middle] |= (byte) (1 << shift);
                yield return new ByteValuesFixture {Values = values};
            }
        }

        private static readonly Lazy<object[]> LazyValues = new Lazy<object[]>(
            () => GetValuesFixtures<ByteValuesFixture, byte>(GetValuesFixtures));

        internal ByteValuesValuesAttribute()
            : base(LazyValues.Value)
        {
        }
    }
}
