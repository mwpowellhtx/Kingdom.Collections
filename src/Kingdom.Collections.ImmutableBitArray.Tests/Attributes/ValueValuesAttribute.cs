using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using Xunit;

    public class ValueValuesAttribute : CombinatorialValuesAttribute
    {
        private static IEnumerable<object> GetValueValues()
        {
            yield return false;
            yield return true;
        }

        private static Lazy<object[]> PrivateValues { get; }
            = new Lazy<object[]>(() => GetValueValues().ToArray());

        public ValueValuesAttribute()
            : base(PrivateValues.Value)
        {
        }
    }
}
