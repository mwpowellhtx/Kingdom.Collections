using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    internal class SpecificIntValuesAttribute : ValuesAttribute
    {
        private static IEnumerable<object> GetValues()
        {
            const uint nibble = 0x0101;
            yield return (uint) nibble;
            yield return (uint) nibble << 8;
            yield return (uint) nibble << 16;
            yield return (uint) nibble << 24;
        }

        private static readonly Lazy<object[]> LazyValues
            = new Lazy<object[]>(() => GetValues().ToArray());

        internal SpecificIntValuesAttribute()
            : base(LazyValues.Value)
        {
        }
    }
}
