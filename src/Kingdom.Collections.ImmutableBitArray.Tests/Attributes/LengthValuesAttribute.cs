using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Kingdom.Collections
{
    using CombinatorialValuesAttribute = ValuesAttribute; // xunit bridge

    public class LengthValuesAttribute : CombinatorialValuesAttribute
    {
        private static IEnumerable<object> GetLengthValues()
        {
            // Int32.MaxValue was not a great idea. I'm not even that sure that Int16.MaxValue is, either.
            const int max = short.MaxValue;
            // Indeed, Int16.MaxValue is much, MUCH faster than Int32.MaxValue would ever be.
            const int middle = max / 2;
            yield return 1;
            yield return 2;
            yield return middle - 1;
            yield return middle;
            yield return middle + 1;
            yield return max - 2;
            yield return max - 1;
        }

        private static readonly Lazy<object[]> LazyValues
            = new Lazy<object[]>(() => GetLengthValues().ToArray());

        public LengthValuesAttribute()
            : base(LazyValues.Value)
        {
        }
    }
}
