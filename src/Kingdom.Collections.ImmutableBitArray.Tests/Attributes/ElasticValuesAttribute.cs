using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    internal class ElasticValuesAttribute : ValuesAttribute
    {
        private static IEnumerable<object> GetValues()
        {
            yield return true;
            yield return false;
        }

        private static readonly Lazy<object[]> LazyValues
            = new Lazy<object[]>(() => GetValues().ToArray());

        internal ElasticValuesAttribute()
            : base(LazyValues.Value)
        {
        }
    }
}
