using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    public class BoolValuesValuesAttribute : ValuesValuesAttributeBase
    {
        private static IEnumerable<BoolValuesFixture> GetValuesFixtures(int length)
        {
            var values = Enumerable.Range(0, length).Select(_ => default(bool)).ToArray();
            if (length > 2)
            {
                values[values.Length / 2] = true;
            }
            yield return new BoolValuesFixture {Values = values};
        }

        private static Lazy<object[]> LazyValues { get; } = new Lazy<object[]>(
            () => GetValuesFixtures<BoolValuesFixture, bool>(GetValuesFixtures));

        internal BoolValuesValuesAttribute()
            : base(LazyValues.Value)
        {
        }
    }
}
