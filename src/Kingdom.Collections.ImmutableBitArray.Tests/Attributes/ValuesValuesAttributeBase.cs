using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Kingdom.Collections
{
    using CombinatorialValuesAttribute = ValuesAttribute; // xunit bridge

    public abstract class ValuesValuesAttributeBase : CombinatorialValuesAttribute
    {
        private static int GetMaximumShift(int value)
        {
            var shift = 0;
            while (1 << ++shift < value)
            {
            }

            return shift - 1;
        }

        private static IEnumerable<int> GetShifts()
        {
            const int max = short.MaxValue;
            const int middle = max / 2;
            yield return 2;
            yield return GetMaximumShift(middle);
            yield return GetMaximumShift(max);
        }

        private static readonly Lazy<IEnumerable<int>> LazyShifts
            = new Lazy<IEnumerable<int>>(GetShifts);

        private static IEnumerable<int> Shifts => LazyShifts.Value;

        protected static object[] GetValuesFixtures<TFixture, TValue>(
            Func<int, IEnumerable<TFixture>> factory)
            where TFixture : ValuesFixtureBase<TValue>
        {
            // Edge use cases and middle.
            return Shifts.SelectMany(s =>
                factory(1 << (s - 1))
                    .Concat(factory(1 << s))
                    .Concat(factory(1 << (s + 1)))).ToArray<object>();
        }

        protected ValuesValuesAttributeBase(params object[] values)
            : base(values)
        {
        }
    }
}
