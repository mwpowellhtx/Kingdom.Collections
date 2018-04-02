using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    public class UInt32ValuesValuesAttribute : ValuesValuesAttributeBase
    {
        private static IEnumerable<UInt32ValuesFixture> GetValuesFixtures(int length)
        {
            var values = Enumerable.Range(0, length).Select(_ => default(uint)).ToArray();
            if (length <= 2)
            {
                yield return new UInt32ValuesFixture {Values = values};
                yield break;
            }

            var middle = values.Length / 2;
            const int maxShifts = sizeof(uint) * 8;
            for (var shift = 0; shift < maxShifts; shift += 2)
            {
                values[middle] |= (uint) (1 << shift);
                yield return new UInt32ValuesFixture {Values = values};
            }
        }

        private static Lazy<object[]> LazyValues { get; } = new Lazy<object[]>(
            () => GetValuesFixtures<UInt32ValuesFixture, uint>(GetValuesFixtures));

        internal UInt32ValuesValuesAttribute()
            : base(LazyValues.Value)
        {
        }
    }
}
